using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

<<<<<<< HEAD:Assets/Namo'sFolder/Script/ShopManager.cs
    private const int ShopSize = 8;
    private const int BaseRankMin = 1;
    private const int BaseRankMax = 10;
    private const int BaseEnchantPower = 6;
    private const int MaxSuitsPerCard = 6;
    private const int PricePerSuit = 3;
=======
    [Header("Shop Positions")]
    public Transform[] cardSpawnPositions = new Transform[8];
>>>>>>> 7eabcef9c22098e716967eff4cf76a423e578b34:Assets/Best'sFolder/ShopManager.cs

    [Header("Shop Positions (Transforms for 8 card slots)")]
    public Transform[] cardSpawnPositions = new Transform[ShopSize];

    [Header("Shop Data (Runtime)")]
    public List<GameObject> shopCards = new List<GameObject>();
    public List<int> cardPrices = new List<int>();

    [Header("Economy")]
    [Tooltip("Player currency for shop purchases")]
    public int playerGold = 100;
    public int startingGold = 100;

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
        if (playerGold == 0) playerGold = startingGold;
        GenerateShopItems();
    }

    private int GetDifficultyLevel()
    {
        // EncounterManager lives in friend folder - access via reflection-safe null check
        // Keep string-based fallback if instance not found to avoid hard compile dep
        var em = EncounterManager.instance;
        return em != null ? em.DificaultyLevel : 0;
    }

    [ContextMenu("Generate Shop Items")]
    public void GenerateShopItems()
    {
        ClearShop();

<<<<<<< HEAD:Assets/Namo'sFolder/Script/ShopManager.cs
        if (CardManager.instance == null)
        {
            Debug.LogWarning("[ShopManager] CardManager.instance is null - cannot generate cards.", this);
            return;
        }

        int di = GetDifficultyLevel();
        int diHalf = di / 2;

        int minRank = BaseRankMin + diHalf;
        int maxRank = BaseRankMax + diHalf;

        // 1. Random ranks for batch
        List<int> generatedRanks = new List<int>(ShopSize);
        for (int i = 0; i < ShopSize; i++)
=======
        int di = EncounterManager.instance != null ? EncounterManager.instance.DificaultyLevel : 0;

        int minRank = 1 + (di / 2);
        int maxRank = 10 + (di / 2);

        List<int> generatedRanks = new List<int>();
        for (int i = 0; i < 8; i++)
>>>>>>> 7eabcef9c22098e716967eff4cf76a423e578b34:Assets/Best'sFolder/ShopManager.cs
        {
            generatedRanks.Add(Random.Range(minRank, maxRank + 1));
        }

        int lowestRank = generatedRanks.Count > 0 ? generatedRanks.Min() : minRank;
        bool lowestRankEnchantedAssigned = false;

<<<<<<< HEAD:Assets/Namo'sFolder/Script/ShopManager.cs
        // Use default curve if null
        if (entranceCurve == null || entranceCurve.length == 0)
            entranceCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
=======
        for (int i = 0; i < 8; i++)
        {
            Vector3 spawnPos = (cardSpawnPositions.Length > i && cardSpawnPositions[i] != null)
                ? cardSpawnPositions[i].position
                : transform.position;
>>>>>>> 7eabcef9c22098e716967eff4cf76a423e578b34:Assets/Best'sFolder/ShopManager.cs

        var spawnedGOs = new List<GameObject>(ShopSize);
        var spawnPositions = new List<Vector3>(ShopSize);
        var spawnRotations = new List<Quaternion>(ShopSize);

        // 2. Spawn cards (hidden, non-interactable initially)
        for (int i = 0; i < ShopSize; i++)
        {
            Vector3 spawnPos = GetSpawnPosition(i);
            Quaternion spawnRot = GetSpawnRotation(i);
            int currentRank = generatedRanks[i];

            List<Suit> cardSuits = CalculateSuitsForCard(currentRank, di, lowestRank, ref lowestRankEnchantedAssigned);

<<<<<<< HEAD:Assets/Namo'sFolder/Script/ShopManager.cs
            GameObject cardObjGO = null;
            try
=======
            GameObject cardObjGO = CardManager.instance.createObj(currentRank, spawnPos, cardSuits);
            CardObj cardObj = cardObjGO.GetComponent<CardObj>();
            Card cardData = cardObj.card;

            // FIX MID-AIR GLITCH: Assign Belong and rotation explicitly
            cardObj.Belong = spawnPos;
            if (cardSpawnPositions.Length > i && cardSpawnPositions[i] != null)
>>>>>>> 7eabcef9c22098e716967eff4cf76a423e578b34:Assets/Best'sFolder/ShopManager.cs
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
            cardObj.canmove = true;
            cardObj.ResetPos();

<<<<<<< HEAD:Assets/Namo'sFolder/Script/ShopManager.cs
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
                // Start as non-selectable until entrance finishes
                cardObj.selectable = false;

                var input = cardObjGO.GetComponent<ShopCardInput>();
                if (input == null) input = cardObjGO.AddComponent<ShopCardInput>();
                input.shopIndex = i;
                input.SetEntranceState(false);

                // Prepare entrance start state: lower + smaller
                cardObjGO.transform.SetPositionAndRotation(spawnPos + Vector3.up * entranceYOffset, spawnRot);
                cardObjGO.transform.localScale = Vector3.one * entranceScaleFrom;
            }

            int suitCount = cardSuits != null ? cardSuits.Count : 1;
            int randomOffset = Random.Range(-3, 4);
            int calculatedPrice = currentRank + (suitCount * PricePerSuit) + randomOffset + diHalf;
=======
            // Attach 3D click listener
            ShopCardClick clicker = cardObjGO.GetComponent<ShopCardClick>();
            if (clicker == null) clicker = cardObjGO.AddComponent<ShopCardClick>();
            clicker.cardIndex = i;

            // Calculate price
            int power = cardData.Rank;
            int suitCount = cardData.suits.Count;
            int randomOffset = Random.Range(-3, 4);

            int calculatedPrice = power + (suitCount * 3) + randomOffset + (di / 2);
>>>>>>> 7eabcef9c22098e716967eff4cf76a423e578b34:Assets/Best'sFolder/ShopManager.cs
            calculatedPrice = Mathf.Max(1, calculatedPrice);

            shopCards.Add(cardObjGO);
            cardPrices.Add(calculatedPrice);
<<<<<<< HEAD:Assets/Namo'sFolder/Script/ShopManager.cs
            spawnedGOs.Add(cardObjGO);
            spawnPositions.Add(spawnPos);
            spawnRotations.Add(spawnRot);
=======

            cardObj.Show();
>>>>>>> 7eabcef9c22098e716967eff4cf76a423e578b34:Assets/Best'sFolder/ShopManager.cs
        }

        // 3. Sequential entrance animation
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
                // Each card animates with slight stagger
                StartCoroutine(AnimateSingleCard(go, targetPos[i], targetRot[i]));
                // Subtle shop SFX tick per card
                if (SoundManager.Instance != null && i % 2 == 0)
                    SoundManager.Instance.PlayInteractCardSFX();
            }
            yield return new WaitForSeconds(staggerDelay);
        }
        // Wait for last card to finish
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
        Vector3 endScale = Vector3.one;
        Quaternion startRot = go.transform.rotation;
        Quaternion endRot = targetRot;
        float elapsed = 0f;

        // Use unscaled? Keep scaled deltaTime
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
        go.transform.localScale = Vector3.one;

        // Enable interaction only after appear completes
        var input = go.GetComponent<ShopCardInput>();
        if (input != null) input.SetEntranceState(true);

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
<<<<<<< HEAD:Assets/Namo'sFolder/Script/ShopManager.cs
        var selectedSuits = new List<Suit>();
        var allSuits = CardManager.instance != null ? CardManager.instance.allsuits : null;

        if (allSuits == null || allSuits.Count == 0)
=======
        List<Suit> selectedSuits = new List<Suit>();
        if (CardManager.instance.allsuits == null || CardManager.instance.allsuits.Count == 0)
>>>>>>> 7eabcef9c22098e716967eff4cf76a423e578b34:Assets/Best'sFolder/ShopManager.cs
            return selectedSuits;

        // Base suit: first 4 are standard, fallback if less than 4
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

        // Grant extra special suits (index >=4) with 50% continuation chance, capped at 6 total
        if (isEnchanted && allSuits.Count > 4)
        {
            int currentSuitCount = selectedSuits.Count;
            bool continueLoop = true;

            while (continueLoop && currentSuitCount < MaxSuitsPerCard)
            {
<<<<<<< HEAD:Assets/Namo'sFolder/Script/ShopManager.cs
                Suit extraSuit = allSuits[Random.Range(4, allSuits.Count)];
                // Avoid duplicate suits
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
=======
                Suit extraSuit = CardManager.instance.allsuits[Random.Range(4, CardManager.instance.allsuits.Count)];
                if (selectedSuits.Contains(extraSuit))
                {
                    continue;
                }
                selectedSuits.Add(extraSuit);
                currentSuitCount++;
>>>>>>> 7eabcef9c22098e716967eff4cf76a423e578b34:Assets/Best'sFolder/ShopManager.cs

                if (Random.value > 0.50f)
                    continueLoop = false;
            }
        }

        return selectedSuits;
    }

    // --- Economy API ---

    public bool CanAfford(int index)
    {
        if (index < 0 || index >= cardPrices.Count) return false;
        if (shopCards.Count <= index || shopCards[index] == null) return false;
        return playerGold >= cardPrices[index];
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

    // --- Purchase ---

    /// <summary>
    /// Purchase with currency validation. Returns true on success.
    /// Triggers shake/SFX feedback.
    /// </summary>
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
        if (playerGold < price)
        {
            Debug.Log($"[ShopManager] Not enough gold: need {price}, have {playerGold}", this);
            // Feedback for failed purchase
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

        // Deduct gold
        playerGold -= price;
        playerGold = Mathf.Max(0, playerGold);

        // Transfer to Tavern (friend folder - keep null-safe)
        if (Tavern.instance != null)
        {
            Tavern.instance.AddCardToDeck(cardObj);
        }
        else
        {
            Debug.LogWarning("[ShopManager] Tavern.instance is null - card not added to deck", this);
        }

        // Success feedback
        if (SoundManager.Instance != null) SoundManager.Instance.PlayInteractCardSFX();
        var camShake = FindAnyObjectByType<CameraShake>();
        if (camShake != null) camShake.Shake(0.12f, 2f, 0.2f);
        var objShaker = cardGO.GetComponent<ObjectShaker>();
        if (objShaker != null) objShaker.Shake(0.1f, 3f, 0.25f);

        // Clear slot (keep index stable for UI, set to null rather than RemoveAt)
        shopCards[index] = null;
        cardPrices[index] = 0;

        // Remove world object from scene - Tavern.Hide() already handles visual, but keep GO if Tavern manages it
        // Do not Destroy; Tavern keeps reference. Just hide from shop view.

        OnCardPurchased?.Invoke(index);

        // If focus manager is showing this card, exit focus
        if (ShopFocusManager.Instance != null && ShopFocusManager.Instance.FocusedIndex == index)
        {
            ShopFocusManager.Instance.ExitFocus();
        }

        return true;
    }

    /// <summary>
    /// Legacy API - now routes through TryBuyCard with validation.
    /// Kept for backward compatibility.
    /// </summary>
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
        StopAllCoroutines(); // stops any per-card AnimateSingleCard still running

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

    // Helper for inspector validation
    private void OnValidate()
    {
        if (cardSpawnPositions == null || cardSpawnPositions.Length != ShopSize)
        {
            Debug.LogWarning($"[ShopManager] cardSpawnPositions should have exactly {ShopSize} elements", this);
        }
    }
}
