using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("Shop Positions (ลากจุด Transform วางการ์ด 8 จุดมาใส่)")]
    public Transform[] cardSpawnPositions = new Transform[8]; // จุดเกิดของการ์ดทั้ง 8 ใบ

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
        // 1. เคลียร์การ์ดเก่าใน Shop (ถ้ามี)
        ClearShop();

        int di = EncounterManager.instance != null ? EncounterManager.instance.DificaultyLevel : 0;
        
        // คำนวณช่วงของ Rank: (1 + DI/2) ถึง (10 + DI/2)
        int minRank = 1 + (di / 2);
        int maxRank = 10 + (di / 2);

        // 2. สุ่ม Rank การ์ดทั้ง 8 ใบ
        List<int> generatedRanks = new List<int>();
        for (int i = 0; i < 8; i++)
        {
            generatedRanks.Add(Random.Range(minRank, maxRank + 1));
        }
        int lowestRank = generatedRanks.Min();
        bool lowestRankEnchantedAssigned = false;

        // 3. วางการ์ดตามตำแหน่ง Transform ทั้ง 8 จุด
        for (int i = 0; i < 8; i++)
        {
            // กำหนดตำแหน่งเกิดจากการ์ดตาม Index ของ Transform Array
            Vector3 spawnPos = (cardSpawnPositions.Length > i && cardSpawnPositions[i] != null) 
                ? cardSpawnPositions[i].position 
                : transform.position;

            int currentRank = generatedRanks[i];

            // คำนวณ Suits และ Enchanted Logic
            List<Suit> cardSuits = CalculateSuitsForCard(currentRank, di, lowestRank, ref lowestRankEnchantedAssigned);

            // 4. สร้างการ์ดผ่าน CardManager
            GameObject cardObjGO = CardManager.instance.createObj(currentRank, spawnPos, cardSuits);
            CardObj cardObj = cardObjGO.GetComponent<CardObj>();
            Card cardData = cardObj.card;

            // ตั้งค่ามุมหมุนของการ์ดให้ตรงกับ Transform จุดเกิด
            if (cardSpawnPositions.Length > i && cardSpawnPositions[i] != null)
            {
                cardObjGO.transform.rotation = cardSpawnPositions[i].rotation;
                cardObj.BelongR = cardSpawnPositions[i].rotation.eulerAngles;
            }

            // 5. คำนวณราคาตามสูตร Mindmap
            int power = cardData.Rank;
            int suitCount = cardData.suits.Count;
            int randomOffset = Random.Range(-3, 4); 
            
            int calculatedPrice = power + (suitCount * 3) + randomOffset + (di / 2);
            calculatedPrice = Mathf.Max(1, calculatedPrice);

            // บันทึกข้อมูล
            shopCards.Add(cardObjGO);
            cardPrices.Add(calculatedPrice);

            // แสดงการ์ด
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