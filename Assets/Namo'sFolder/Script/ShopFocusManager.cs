using UnityEngine;
using TMPro;

public class ShopFocusManager : MonoBehaviour
{
    public static ShopFocusManager Instance;

    [Header("Camera Control")]
    public float moveSpeed = 5f;
    public float focusOffsetZ = -2f;

    [Header("UI Focus Elements")]
    public GameObject focusUIPanel;
    public TextMeshProUGUI cardInfoText;
    public TextMeshProUGUI cardPriceText;

    private Vector3 targetCamPos;

    private void Awake()
    {
        Instance = this;
        targetCamPos = transform.position;
    }

    private void Update()
    {
        // กล้องสไลด์นุ่มนวลไปยังตำแหน่งเป้าหมาย
        transform.position = Vector3.Lerp(transform.position, targetCamPos, Time.deltaTime * moveSpeed);
    }

    // เรียกฟังก์ชันนี้เมื่อคลิกการ์ด (ส่ง Index 0 - 7 เข้ามา)
    public void FocusOnCard(int index)
    {
        if (ShopManager.Instance == null || index < 0 || index >= ShopManager.Instance.shopCards.Count) return;
        if (ShopManager.Instance.shopCards[index] == null) return;

        // 1. ซ่อนการ์ดใบอื่น ให้เห็นเฉพาะใบที่เลือก
        for (int i = 0; i < ShopManager.Instance.shopCards.Count; i++)
        {
            if (ShopManager.Instance.shopCards[i] != null)
            {
                ShopManager.Instance.shopCards[i].SetActive(i == index);
            }
        }

        // 2. ตั้งตำแหน่งกล้องให้เลื่อนไปหาการ์ดใบนั้น
        Vector3 cardPos = ShopManager.Instance.shopCards[index].transform.position;
        targetCamPos = new Vector3(cardPos.x, cardPos.y, cardPos.z + focusOffsetZ);

        // 3. แสดง UI และดึงราคาจาก ShopManager
        if (focusUIPanel != null) focusUIPanel.SetActive(true);

        GameObject currentCardGO = ShopManager.Instance.shopCards[index];
        CardObj cardObj = currentCardGO.GetComponent<CardObj>();

        if (cardInfoText != null && cardObj != null && cardObj.card != null)
        {
            cardInfoText.text = $"{cardObj.card.cardname}\nRank: {cardObj.card.Rank}";
        }

        if (cardPriceText != null)
        {
            int price = ShopManager.Instance.cardPrices[index];
            cardPriceText.text = $"{price} Coins";
        }
    }
}