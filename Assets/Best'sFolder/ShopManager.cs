using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("Shop Positions")]
    public Transform[] cardSpawnPositions = new Transform[8];

    [Header("Shop Data")]
    public List<GameObject> shopCards = new List<GameObject>();
    public List<int> cardPrices = new List<int>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GenerateShopItems();
    }

    [ContextMenu("Generate Shop Items")]
    public void GenerateShopItems()
    {
        ClearShop();

        int di = EncounterManager.instance != null ? EncounterManager.instance.DificaultyLevel : 0;

        int minRank = 1 + (di / 2);
        int maxRank = 10 + (di / 2);

        List<int> generatedRanks = new List<int>();
        for (int i = 0; i < 8; i++)
        {
            generatedRanks.Add(Random.Range(minRank, maxRank + 1));
        }
        int lowestRank = generatedRanks.Min();
        bool lowestRankEnchantedAssigned = false;

        for (int i = 0; i < 8; i++)
        {
            Vector3 spawnPos = (cardSpawnPositions.Length > i && cardSpawnPositions[i] != null)
                ? cardSpawnPositions[i].position
                : transform.position;

            int currentRank = generatedRanks[i];

            List<Suit> cardSuits = CalculateSuitsForCard(currentRank, di, lowestRank, ref lowestRankEnchantedAssigned);

            GameObject cardObjGO = CardManager.instance.createObj(currentRank, spawnPos, cardSuits);
            CardObj cardObj = cardObjGO.GetComponent<CardObj>();
            Card cardData = cardObj.card;

            // FIX MID-AIR GLITCH: Assign Belong and rotation explicitly
            cardObj.Belong = spawnPos;
            if (cardSpawnPositions.Length > i && cardSpawnPositions[i] != null)
            {
                cardObjGO.transform.rotation = cardSpawnPositions[i].rotation;
                cardObj.BelongR = cardSpawnPositions[i].rotation.eulerAngles;
            }
            cardObj.canmove = true;
            cardObj.ResetPos();

            // Attach 3D click listener
            ShopCardClick clicker = cardObjGO.GetComponent<ShopCardClick>();
            if (clicker == null) clicker = cardObjGO.AddComponent<ShopCardClick>();
            clicker.cardIndex = i;

            // Calculate price
            int power = cardData.Rank;
            int suitCount = cardData.suits.Count;
            int randomOffset = Random.Range(-3, 4);

            int calculatedPrice = power + (suitCount * 3) + randomOffset + (di / 2);
            calculatedPrice = Mathf.Max(1, calculatedPrice);

            shopCards.Add(cardObjGO);
            cardPrices.Add(calculatedPrice);

            cardObj.Show();
        }
    }

    private List<Suit> CalculateSuitsForCard(int rank, int di, int lowestRank, ref bool lowestAssigned)
    {
        List<Suit> selectedSuits = new List<Suit>();
        if (CardManager.instance.allsuits == null || CardManager.instance.allsuits.Count == 0)
            return selectedSuits;

        selectedSuits.Add(CardManager.instance.allsuits[Random.Range(0, 4)]);

        int maxAllowedPower = 6 + (di / 2);
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

        if (isEnchanted && CardManager.instance.allsuits.Count > 4)
        {
            int currentSuitCount = 1;
            bool continueLoop = true;

            while (continueLoop && currentSuitCount < 6)
            {
                Suit extraSuit = CardManager.instance.allsuits[Random.Range(4, CardManager.instance.allsuits.Count)];
                if (selectedSuits.Contains(extraSuit))
                {
                    continue;
                }
                selectedSuits.Add(extraSuit);
                currentSuitCount++;

                if (Random.value > 0.50f)
                {
                    continueLoop = false;
                }
            }
        }

        return selectedSuits;
    }

    public bool BuyCard(int index)
    {
        if (index < 0 || index >= shopCards.Count || shopCards[index] == null) return false;

        GameObject cardGO = shopCards[index];
        CardObj cardObj = cardGO.GetComponent<CardObj>();

        if (Tavern.instance != null)
        {
            Tavern.instance.AddCardToDeck(cardObj);
        }

        shopCards[index] = null;
        cardPrices[index] = 0;

        return true;
    }

    public void ClearShop()
    {
        foreach (var card in shopCards)
        {
            if (card != null) Destroy(card);
        }
        shopCards.Clear();
        cardPrices.Clear();
    }
}