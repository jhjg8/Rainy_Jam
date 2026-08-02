using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("Shop Layout Settings")]
    public Transform shopCenterPos;   // จุดศูนย์กลางสำหรับวางการ์ด 4x2
    public float spacingX = 2.5f;     // ระยะห่างแกน X
    public float spacingZ = 3.5f;     // ระยะห่างแกน Z

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

        // 2. สุ่ม Rank การ์ดทั้ง 8 ใบเพื่อหาใบที่ Rank ต่ำที่สุด
        List<int> generatedRanks = new List<int>();
        for (int i = 0; i < 8; i++)
        {
            generatedRanks.Add(Random.Range(minRank, maxRank + 1));
        }
        int lowestRank = generatedRanks.Min();
        bool lowestRankEnchantedAssigned = false;

        // 3. วางการ์ดแบบ 4x2 (4 คอลัมน์, 2 แถว ในแกน X และ Z)
        for (int i = 0; i < 8; i++)
        {
            int row = i / 4; // 0 หรือ 1
            int col = i % 4; // 0, 1, 2, 3

            // คำนวณตำแหน่ง Grid Offset
            float posX = (col - 1.5f) * spacingX;
            float posZ = (row - 0.5f) * spacingZ;
            Vector3 spawnPos = shopCenterPos != null 
                ? shopCenterPos.position + new Vector3(posX, 0, posZ) 
                : new Vector3(posX, 0, posZ);

            int currentRank = generatedRanks[i];

            // --- คำนวณ Suits และ Enchanted Logic ---
            List<Suit> cardSuits = CalculateSuitsForCard(currentRank, di, lowestRank, ref lowestRankEnchantedAssigned);

            // 4. สร้างการ์ดผ่าน CardManager
            GameObject cardObjGO = CardManager.instance.createObj(currentRank, spawnPos, cardSuits);
            CardObj cardObj = cardObjGO.GetComponent<CardObj>();
            Card cardData = cardObj.card;

            // 5. คำนวณราคาตามสูตร Mindmap
            // Price = Power + (Number of suits * 3) + Random(-3 to 3) + (DI / 2) [ไม่ต่ำกว่า 1]
            int power = cardData.Rank;
            int suitCount = cardData.suits.Count;
            int randomOffset = Random.Range(-3, 4); // -3 ถึง 3
            
            int calculatedPrice = power + (suitCount * 3) + randomOffset + (di / 2);
            calculatedPrice = Mathf.Max(1, calculatedPrice); // Price (Never less than 1)

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

        // Suit แรก (Standard Suit: เลือกจาก 0-3)
        selectedSuits.Add(CardManager.instance.allsuits[Random.Range(0, 4)]);

        // ตรวจสอบเงื่อนไขการติด Enchanted:
        // - ต้องเป็นการ์ดที่มี Power <= 6 + (DI / 2)
        // - การ์ดที่มี Rank ต่ำสุด จะได้สิทธิ์ 100% ในการติด Enchanted (1 ใบ)
        int maxAllowedPower = 6 + (di / 2);
        bool isEnchanted = false;

        if (rank <= maxAllowedPower)
        {
            if (rank == lowestRank && !lowestAssigned)
            {
                isEnchanted = true;
                lowestAssigned = true; // ล็อคให้การ์ด Rank ต่ำสุดใบแรกได้สิทธิ์ 100%
            }
            else
            {
                // โอกาสติด 25% สำหรับใบปกติที่เข้าเงื่อนไข
                isEnchanted = Random.value <= 0.25f;
            }
        }

        // สุ่มเพิ่ม Enchant Suits (สูงสุดไม่เกิน 6 Suits รวม Standard)
        if (isEnchanted && CardManager.instance.allsuits.Count > 4)
        {
            int currentSuitCount = 1;
            bool continueLoop = true;

            while (continueLoop && currentSuitCount < 6)
            {
                // สุ่ม Suit พิเศษ (ดึงตั้งแต่ index 4 เป็นต้นไป)
                Suit extraSuit = CardManager.instance.allsuits[Random.Range(4, CardManager.instance.allsuits.Count)];
                selectedSuits.Add(extraSuit);
                currentSuitCount++;

                // โอกาส 50% สำหรับ Special Suit ถัดไป
                if (Random.value > 0.50f)
                {
                    continueLoop = false;
                }
            }
        }

        return selectedSuits;
    }

    // ฟังก์ชันสำหรับซื้อการ์ด
    public bool BuyCard(int index)
    {
        if (index < 0 || index >= shopCards.Count || shopCards[index] == null) return false;

        GameObject cardGO = shopCards[index];
        CardObj cardObj = cardGO.GetComponent<CardObj>();

        // ส่งการ์ดเข้า Deck/Hand ของผู้เล่น
        if (Tavern.instance != null)
        {
            Tavern.instance.AddCardToDeck(cardObj);
        }

        // ลบออกจาก Shop
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