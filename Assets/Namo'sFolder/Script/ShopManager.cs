using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    private const int ShopSize = 8;
    private const int BaseRankMin = 1;
    private const int BaseRankMax = 10;
    private const int BaseEnchantPower = 6;
    private const int MaxSuitsPerCard = 6;
    private const int PricePerSuit = 3;

    [Header("Shop Positions (Transforms for 8 card slots)")]
    public Transform[] cardSpawnPositions = new Transform[ShopSize];

    [Header("Shop Data (Runtime)")]
    public List<GameObject> shopCards = new List<GameObject>();
    public List<int> cardPrices = new List<int>();

    [Header("Economy")]
    [Tooltip("Player currency for shop purchases")]
    public int playerGold = 100;
    public int startingGold = 100;

    [Header("Face Cards Pool (12 = 4 suits x Jack/Queen/King)")]
    [Tooltip("When enabled, shop only spawns Jack/Queen/King of Clubs/Diamonds/Hearts/Spades")]
    public bool useFaceCardsOnly = true;
    [Tooltip("Ranks for face cards: 11=Jack, 12=Queen, 13=King")]
    public int[] faceRanks = new int[] { 11, 12, 13 };
    [Tooltip("If true, ensure 8 cards are unique among the 12 pool (no duplicates)")]
    public bool uniqueFaceCards = false;

    [Header("Price (all same)")]
    public bool useFixedPrice = true;
    public int fixedPrice = 5;

    [Header("Card Appearance (fix 'too big')")]
    [Tooltip("Final scale of shop cards - test scene uses ~0.7, main game was 1.0 too big")]
    [Range(0.3f, 1.2f)] public float shopCardScale = 0.7f;

    [Header("Entrance Animation")]
    [Range(0.02f, 0.2f)] public float staggerDelay = 0.075f;
    public float entranceDuration = 0.38f;
    public float entranceYOffset = -0.7f;
    public float entranceScaleFrom = 0.6f;
    [Tooltip("Easing for entrance pop")]
    public AnimationCurve entranceCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    public System.Action<int> OnCardPurchased;
    public System.Action OnShopGenerated;

    private Coroutine entranceRoutine;
    private bool isEntranceAnimating = false;
    public bool IsEntranceAnimating => isEntranceAnimating;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Keep the higher gold (Inspector 500 won't be overwritten to 0)
        if (CycleManager.Instance != null) playerGold = Mathf.Max(playerGold, CycleManager.Instance.Coins);
        if (playerGold == 0) playerGold = startingGold;
        if (CycleManager.Instance != null) CycleManager.Instance.Coins = playerGold; // sync back

        if (ShopCameraManager.Instance != null && ShopCameraManager.Instance.IsInShop)
            GenerateShopItems();
        else
            ClearShop();
    }

    private void OnEnable()
    {
        if (CycleManager.Instance != null) playerGold = Mathf.Max(playerGold, CycleManager.Instance.Coins);
    }

    private int GetDifficultyLevel()
    {
        var em = EncounterManager.instance;
        return em != null ? em.DificaultyLevel : 0;
    }

    [ContextMenu("Generate Shop Items")]
    public void GenerateShopItems()
    {
        ClearShop();

        if (CardManager.instance == null)
        {
            Debug.LogWarning("[ShopManager] CardManager.instance is null - cannot generate cards.", this);
            return;
        }

        int di = GetDifficultyLevel();
        int diHalf = di / 2;

        // Build 12-pool logic vs DI range logic
        List<int> generatedRanks = new List<int>(ShopSize);
        List<int> generatedSuitIndices = new List<int>(ShopSize); // 0=Club,1=Diamond,2=Heart,3=Spade

        if (useFaceCardsOnly && faceRanks != null && faceRanks.Length > 0)
        {
            // Optional unique sampling without replacement for 8 of 12
            List<Vector2Int> pool = new List<Vector2Int>(); // x=suitIdx, y=rankIdx
            for (int s = 0; s < 4; s++)
                for (int r = 0; r < faceRanks.Length; r++)
                    pool.Add(new Vector2Int(s, r));

            if (uniqueFaceCards) pool = pool.OrderBy(x => Random.value).ToList();

            for (int i = 0; i < ShopSize; i++)
            {
                Vector2Int pick;
                if (uniqueFaceCards && pool.Count > 0)
                {
                    pick = pool[0];
                    pool.RemoveAt(0);
                }
                else
                {
                    int suitIdx = Random.Range(0, 4);
                    int rankIdx = Random.Range(0, faceRanks.Length);
                    pick = new Vector2Int(suitIdx, rankIdx);
                }
                generatedSuitIndices.Add(pick.x);
                generatedRanks.Add(faceRanks[pick.y]);
            }
        }
        else
        {
            int minRank = BaseRankMin + diHalf;
            int maxRank = BaseRankMax + diHalf;
            for (int i = 0; i < ShopSize; i++)
            {
                generatedRanks.Add(Random.Range(minRank, maxRank + 1));
                generatedSuitIndices.Add(-1); // -1 means use CalculateSuitsForCard random base
            }
        }

        int lowestRank = generatedRanks.Count > 0 ? generatedRanks.Min() : (faceRanks.Length > 0 ? faceRanks[0] : 1);
        bool lowestRankEnchantedAssigned = false;

        if (entranceCurve == null || entranceCurve.length == 0)
            entranceCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        var spawnedGOs = new List<GameObject>(ShopSize);
        var spawnPositions = new List<Vector3>(ShopSize);
        var spawnRotations = new List<Quaternion>(ShopSize);

        for (int i = 0; i < ShopSize; i++)
        {
            Vector3 spawnPos = GetSpawnPosition(i);
            Quaternion spawnRot = GetSpawnRotation(i);
            int currentRank = generatedRanks[i];
            int forcedSuitIdx = (generatedSuitIndices.Count > i) ? generatedSuitIndices[i] : -1;

            List<Suit> cardSuits;
            if (useFaceCardsOnly && forcedSuitIdx >= 0)
            {
                // Force exact suit from 12-pool: single base suit (Club/Diamond/Heart/Spade), no extra enchanted stacking for clean 12
                cardSuits = GetSuitsForFaceCard(forcedSuitIdx);
            }
            else
            {
                cardSuits = CalculateSuitsForCard(currentRank, di, lowestRank, ref lowestRankEnchantedAssigned);
            }

            GameObject cardObjGO = null;
            try
            {
                cardObjGO = CardManager.instance.createObj(currentRank, spawnPos, cardSuits);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[ShopManager] Failed to create card at index {i}: {e.Message}", this);
                shopCards.Add(null);
                cardPrices.Add(0);
                spawnedGOs.Add(null);
                spawnPositions.Add(spawnPos);
                spawnRotations.Add(spawnRot);
                continue;
            }

            if (cardObjGO == null)
            {
                Debug.LogWarning($"[ShopManager] CardManager.createObj returned null at index {i}", this);
                shopCards.Add(null);
                cardPrices.Add(0);
                spawnedGOs.Add(null);
                spawnPositions.Add(spawnPos);
                spawnRotations.Add(spawnRot);
                continue;
            }

            CardObj cardObj = cardObjGO.GetComponent<CardObj>();
            if (cardObj == null)
            {
                Debug.LogWarning($"[ShopManager] Created object missing CardObj at index {i}", this);
            }
            else
            {
                if (cardSpawnPositions.Length > i && cardSpawnPositions[i] != null)
                {
                    cardObj.BelongR = spawnRot.eulerAngles;
                    cardObj.Belong = spawnPos;
                }
                cardObj.Show();
                cardObj.selectable = false;

                var input = cardObjGO.GetComponent<ShopCardInput>();
                if (input == null) input = cardObjGO.AddComponent<ShopCardInput>();
                input.shopIndex = i;
                input.SetEntranceState(false);

                cardObjGO.transform.SetPositionAndRotation(spawnPos + Vector3.up * entranceYOffset, spawnRot);
                cardObjGO.transform.localScale = Vector3.one * entranceScaleFrom;
            }

            int calculatedPrice;
            if (useFixedPrice) calculatedPrice = Mathf.Max(1, fixedPrice);
            else
            {
                int suitCount = cardSuits != null ? cardSuits.Count : 1;
                int randomOffset = Random.Range(-3, 4);
                calculatedPrice = currentRank + (suitCount * PricePerSuit) + randomOffset + diHalf;
                calculatedPrice = Mathf.Max(1, calculatedPrice);
            }

            shopCards.Add(cardObjGO);
            cardPrices.Add(calculatedPrice);
            spawnedGOs.Add(cardObjGO);
            spawnPositions.Add(spawnPos);
            spawnRotations.Add(spawnRot);
        }

        if (entranceRoutine != null) StopCoroutine(entranceRoutine);
        entranceRoutine = StartCoroutine(EntranceSequence(spawnedGOs, spawnPositions, spawnRotations));

        OnShopGenerated?.Invoke();
    }

    private System.Collections.IEnumerator EntranceSequence(List<GameObject> gos, List<Vector3> targetPos, List<Quaternion> targetRot)
    {
        isEntranceAnimating = true;
        for (int i = 0; i < gos.Count; i++)
        {
            var go = gos[i];
            if (go != null)
            {
                StartCoroutine(AnimateSingleCard(go, targetPos[i], targetRot[i]));
                if (SoundManager.Instance != null && i % 2 == 0)
                    SoundManager.Instance.PlayInteractCardSFX();
            }
            yield return new WaitForSeconds(staggerDelay);
        }
        yield return new WaitForSeconds(entranceDuration);
        isEntranceAnimating = false;
        entranceRoutine = null;
    }

    private System.Collections.IEnumerator AnimateSingleCard(GameObject go, Vector3 targetPos, Quaternion targetRot)
    {
        if (go == null) yield break;
        Vector3 startPos = go.transform.position;
        Vector3 endPos = targetPos;
        Vector3 startScale = go.transform.localScale;
        Vector3 endScale = Vector3.one * shopCardScale;
        Quaternion startRot = go.transform.rotation;
        Quaternion endRot = targetRot;
        float elapsed = 0f;

        while (elapsed < entranceDuration)
        {
            if (go == null) yield break;
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / entranceDuration);
            float curveT = entranceCurve != null ? entranceCurve.Evaluate(t) : Mathf.SmoothStep(0f, 1f, t);

            go.transform.position = Vector3.LerpUnclamped(startPos, endPos, curveT);
            go.transform.localScale = Vector3.LerpUnclamped(startScale, endScale, curveT);
            go.transform.rotation = Quaternion.SlerpUnclamped(startRot, endRot, curveT);
            yield return null;
        }
        if (go == null) yield break;
        go.transform.SetPositionAndRotation(endPos, endRot);
        go.transform.localScale = Vector3.one * shopCardScale;

        var input = go.GetComponent<ShopCardInput>();
        if (input != null)
        {
            input.SetEntranceState(true);
            input.RefreshBase(); // update hover base to new scaled size
        }

        var cardObj = go.GetComponent<CardObj>();
        if (cardObj != null)
        {
            cardObj.selectable = true;
            cardObj.Belong = endPos;
            cardObj.BelongR = endRot.eulerAngles;
        }
    }

    private Vector3 GetSpawnPosition(int index)
    {
        if (cardSpawnPositions != null && cardSpawnPositions.Length > index && cardSpawnPositions[index] != null)
            return cardSpawnPositions[index].position;
        return transform.position;
    }

    private Quaternion GetSpawnRotation(int index)
    {
        if (cardSpawnPositions != null && cardSpawnPositions.Length > index && cardSpawnPositions[index] != null)
            return cardSpawnPositions[index].rotation;
        return transform.rotation;
    }

    private List<Suit> CalculateSuitsForCard(int rank, int di, int lowestRank, ref bool lowestAssigned)
    {
        var selectedSuits = new List<Suit>();
        var allSuits = CardManager.instance != null ? CardManager.instance.allsuits : null;

        if (allSuits == null || allSuits.Count == 0)
            return selectedSuits;

        int baseSuitCount = Mathf.Min(4, allSuits.Count);
        selectedSuits.Add(allSuits[Random.Range(0, baseSuitCount)]);

        int maxAllowedPower = BaseEnchantPower + (di / 2);
        bool isEnchanted = false;

        if (rank <= maxAllowedPower)
        {
            if (rank == lowestRank && !lowestAssigned)
            {
                isEnchanted = true;
                lowestAssigned = true;
            }
            else
            {
                isEnchanted = Random.value <= 0.25f;
            }
        }

        if (isEnchanted && allSuits.Count > 4)
        {
            int currentSuitCount = selectedSuits.Count;
            bool continueLoop = true;

            while (continueLoop && currentSuitCount < MaxSuitsPerCard)
            {
                Suit extraSuit = allSuits[Random.Range(4, allSuits.Count)];
                if (!selectedSuits.Contains(extraSuit))
                {
                    selectedSuits.Add(extraSuit);
                    currentSuitCount++;
                }
                else if (selectedSuits.Count >= allSuits.Count)
                {
                    break;
                }
                else
                {
                    continue;
                }

                if (Random.value > 0.50f)
                    continueLoop = false;
            }
        }

        return selectedSuits;
    }

    private List<Suit> GetSuitsForFaceCard(int suitIndex)
    {
        var list = new List<Suit>();
        var allSuits = CardManager.instance != null ? CardManager.instance.allsuits : null;
        if (allSuits == null || allSuits.Count == 0) return list;
        int idx = Mathf.Clamp(suitIndex, 0, Mathf.Min(3, allSuits.Count - 1));
        list.Add(allSuits[idx]); // 0:Club, 1:Diamond, 2:Heart, 3:Spade
        return list;
    }

    public bool CanAfford(int index)
    {
        if (index < 0 || index >= cardPrices.Count) return false;
        if (shopCards.Count <= index || shopCards[index] == null) return false;
        // Use max of both gold systems so setting either to 500 works (fixes "500 but can't buy")
        int gold = playerGold;
        if (CycleManager.Instance != null) gold = Mathf.Max(gold, CycleManager.Instance.Coins);
        return gold >= cardPrices[index];
    }

    public int GetPrice(int index)
    {
        if (index < 0 || index >= cardPrices.Count) return -1;
        return cardPrices[index];
    }

    public int GetCardIndex(GameObject cardGO)
    {
        if (cardGO == null) return -1;
        return shopCards.IndexOf(cardGO);
    }

    public void AddGold(int amount)
    {
        playerGold = Mathf.Max(0, playerGold + amount);
    }

    public bool HasGold(int amount) => playerGold >= amount;

    public bool TryBuyCard(int index)
    {
        if (index < 0 || index >= shopCards.Count)
        {
            Debug.LogWarning($"[ShopManager] TryBuyCard: index {index} out of range", this);
            return false;
        }
        if (shopCards[index] == null)
        {
            Debug.LogWarning($"[ShopManager] TryBuyCard: card at {index} already sold/null", this);
            return false;
        }
        if (index >= cardPrices.Count)
        {
            Debug.LogWarning($"[ShopManager] TryBuyCard: price array out of sync at {index}", this);
            return false;
        }

        int price = cardPrices[index];
        int curGold = playerGold;
        if (CycleManager.Instance != null) curGold = Mathf.Max(curGold, CycleManager.Instance.Coins);
        Debug.Log($"[Shop] TryBuy price {price} have {curGold} (Cycle={CycleManager.Instance?.Coins} playerGold={playerGold})");
        if (curGold < price)
        {
            Debug.Log($"[ShopManager] Not enough gold: need {price}, have {curGold}", this);
            if (SoundManager.Instance != null) SoundManager.Instance.PlayDamageSFX();
            var shaker = FindAnyObjectByType<CameraShake>();
            if (shaker != null) shaker.Shake(0.08f, 1f, 0.15f);
            return false;
        }

        GameObject cardGO = shopCards[index];
        CardObj cardObj = cardGO != null ? cardGO.GetComponent<CardObj>() : null;

        if (cardObj == null)
        {
            Debug.LogWarning($"[ShopManager] TryBuyCard: CardObj missing at {index}", this);
            return false;
        }

        // Deduct from the pool that actually has gold, then sync both
        if (CycleManager.Instance != null && CycleManager.Instance.Coins >= price)
            CycleManager.Instance.Coins -= price;
        else if (playerGold >= price)
            playerGold -= price;
        else if (CycleManager.Instance != null) // fallback max case
            CycleManager.Instance.Coins = Mathf.Max(0, curGold - price);

        playerGold = Mathf.Max(0, playerGold);
        if (CycleManager.Instance != null) { CycleManager.Instance.Coins = Mathf.Max(CycleManager.Instance.Coins, 0); playerGold = CycleManager.Instance.Coins; }

        if (Tavern.instance != null)
        {
            Tavern.instance.AddCardToDeck(cardObj);
        }
        else
        {
            Debug.LogWarning("[ShopManager] Tavern.instance is null - card not added to deck", this);
        }

        if (SoundManager.Instance != null) SoundManager.Instance.PlayInteractCardSFX();
        var camShake = FindAnyObjectByType<CameraShake>();
        if (camShake != null) camShake.Shake(0.12f, 2f, 0.2f);
        var objShaker = cardGO.GetComponent<ObjectShaker>();
        if (objShaker != null) objShaker.Shake(0.1f, 3f, 0.25f);

        shopCards[index] = null;
        cardPrices[index] = 0;

        OnCardPurchased?.Invoke(index);

        if (ShopFocusManager.Instance != null && ShopFocusManager.Instance.FocusedIndex == index)
        {
            ShopFocusManager.Instance.ExitFocus();
        }

        return true;
    }

    public bool BuyCard(int index)
    {
        return TryBuyCard(index);
    }

    public void ClearShop()
    {
        if (entranceRoutine != null)
        {
            StopCoroutine(entranceRoutine);
            entranceRoutine = null;
            isEntranceAnimating = false;
        }
        StopAllCoroutines();

        for (int i = 0; i < shopCards.Count; i++)
        {
            var card = shopCards[i];
            if (card != null)
            {
                bool isInTavern = Tavern.instance != null && Tavern.instance.card.Contains(card.GetComponent<CardObj>());
                if (!isInTavern)
                    Destroy(card);
            }
        }
        shopCards.Clear();
        cardPrices.Clear();
    }

    private void OnValidate()
    {
        if (cardSpawnPositions == null || cardSpawnPositions.Length != ShopSize)
        {
            Debug.LogWarning($"[ShopManager] cardSpawnPositions should have exactly {ShopSize} elements", this);
        }
    }
}
