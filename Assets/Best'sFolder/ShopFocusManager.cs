using UnityEngine;
using TMPro;
using System.Linq;

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
    public TextMeshProUGUI playerCoinsText; // Drag your Coins TextMeshPro here

    private Vector3 targetCamPos;
    private int focusedIndex = -1;

    private void Awake()
    {
        Instance = this;
        targetCamPos = transform.position;

        if (focusUIPanel != null)
            focusUIPanel.SetActive(false);
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetCamPos, Time.deltaTime * moveSpeed);

        // Display player coins continuously
        if (playerCoinsText != null && CycleManager.Instance != null)
        {
            playerCoinsText.text = $"Coins: {CycleManager.Instance.Coins}";
        }
    }

    public void FocusOnCard(int index)
    {
        if (ShopManager.Instance == null || index < 0 || index >= ShopManager.Instance.shopCards.Count) return;
        if (ShopManager.Instance.shopCards[index] == null) return;

        focusedIndex = index;

        // Hide other cards
        for (int i = 0; i < ShopManager.Instance.shopCards.Count; i++)
        {
            if (ShopManager.Instance.shopCards[i] != null)
            {
                ShopManager.Instance.shopCards[i].SetActive(i == index);
            }
        }

        Vector3 cardPos = ShopManager.Instance.shopCards[index].transform.position;
        targetCamPos = new Vector3(cardPos.x, cardPos.y, cardPos.z + focusOffsetZ);

        if (focusUIPanel != null) focusUIPanel.SetActive(true);

        GameObject currentCardGO = ShopManager.Instance.shopCards[index];
        CardObj cardObj = currentCardGO.GetComponent<CardObj>();

        if (cardInfoText != null && cardObj != null && cardObj.card != null)
        {
            string suitNames = string.Join(", ", cardObj.card.suits.Select(s => s.Name));
            cardInfoText.text = $"{cardObj.card.cardname}\nRank: {cardObj.card.Rank}\nSuits: {suitNames}";
        }

        if (cardPriceText != null)
        {
            int price = ShopManager.Instance.cardPrices[index];
            cardPriceText.text = $"Price: {price} Coins";
        }
    }

    public void BuyFocusedCard()
    {
        if (focusedIndex < 0 || focusedIndex >= ShopManager.Instance.shopCards.Count) return;

        int price = ShopManager.Instance.cardPrices[focusedIndex];
        if (CycleManager.Instance != null && CycleManager.Instance.Coins >= price)
        {
            CycleManager.Instance.Coins -= price;
            ShopManager.Instance.BuyCard(focusedIndex);
            Unfocus();
        }
    }

    public void Unfocus()
    {
        if (focusUIPanel != null) focusUIPanel.SetActive(false);
        focusedIndex = -1;

        if (ShopManager.Instance != null)
        {
            for (int i = 0; i < ShopManager.Instance.shopCards.Count; i++)
            {
                if (ShopManager.Instance.shopCards[i] != null)
                {
                    ShopManager.Instance.shopCards[i].SetActive(true);
                }
            }
        }

        if (ShopCameraManager.Instance != null && ShopCameraManager.Instance.shopCameraPos != null)
        {
            targetCamPos = ShopCameraManager.Instance.shopCameraPos.position;
        }
    }
}