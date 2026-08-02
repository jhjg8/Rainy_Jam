using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopFocusManager : MonoBehaviour
{
    public static ShopFocusManager Instance;

    [Header("Cards Setup")]
    public Card3DInteract[] allCards; // ลาก Card_1, 2, 3 มาใส่

    [Header("Camera Control")]
    public Transform defaultCamPos;  // ลาก DefaultCamPos มาใส่
    public float moveSpeed = 5f;
    public float focusOffsetZ = -2f; // ระยะถอยห่างหน้าการ์ดตอนซูม

    [Header("UI Focus Elements")]
    public GameObject focusUIPanel;
    public TextMeshProUGUI cardInfoText;
    public TextMeshProUGUI cardPriceText;
    public Button leftArrowButton;
    public Button rightArrowButton;

    private int currentIndex = -1;
    private Vector3 targetCamPos;

    private void Awake() => Instance = this;

    private void Start()
    {
        if (defaultCamPos != null) targetCamPos = defaultCamPos.position;
        if (focusUIPanel != null) focusUIPanel.SetActive(false);
    }

    private void Update()
    {
        // กล้องสไลด์นุ่มนวลไปยังตำแหน่งเป้าหมาย
        transform.position = Vector3.Lerp(transform.position, targetCamPos, Time.deltaTime * moveSpeed);
    }

    public void FocusOnCard(int index)
    {
        currentIndex = index;

        // 1. เปิดการ์ดเฉพาะใบที่โฟกัส ซ่อนใบอื่น
        for (int i = 0; i < allCards.Length; i++)
        {
            if (allCards[i] != null)
                allCards[i].gameObject.SetActive(i == currentIndex);
        }

        // 2. ตั้งจุดกล้องสไลด์
        Vector3 cardPos = allCards[currentIndex].transform.position;
        targetCamPos = new Vector3(cardPos.x, cardPos.y, cardPos.z + focusOffsetZ);

        // 3. อัปเดต UI
        UpdateUI();
    }

    public void NextCard()
    {
        if (currentIndex < allCards.Length - 1) FocusOnCard(currentIndex + 1);
    }

    public void PreviousCard()
    {
        if (currentIndex > 0) FocusOnCard(currentIndex - 1);
    }

    public void ExitFocus()
    {
        currentIndex = -1;

        // แสดงการ์ดทุกใบกลับมา
        foreach (var card in allCards)
        {
            if (card != null) card.gameObject.SetActive(true);
        }

        if (defaultCamPos != null) targetCamPos = defaultCamPos.position;
        if (focusUIPanel != null) focusUIPanel.SetActive(false);
    }

    public void BuyCurrentCard()
    {
        if (currentIndex < 0 || allCards[currentIndex] == null) return;

        Card3DInteract currentCard = allCards[currentIndex];
        if (ShopDemoManager.Instance.SpendCoinsWithFX(currentCard.price))
        {
            Destroy(currentCard.gameObject);
            allCards[currentIndex] = null;
            ExitFocus();
        }
    }

    private void UpdateUI()
    {
        if (focusUIPanel != null) focusUIPanel.SetActive(true);

        Card3DInteract card = allCards[currentIndex];
        if (cardInfoText != null) cardInfoText.text = card.cardDescription;
        if (cardPriceText != null) cardPriceText.text = $"{card.price} Coins";

        // เปิด/ปิด ลูกศรตามขอบ
        if (leftArrowButton != null) leftArrowButton.gameObject.SetActive(currentIndex > 0);
        if (rightArrowButton != null) rightArrowButton.gameObject.SetActive(currentIndex < allCards.Length - 1);
    }
}