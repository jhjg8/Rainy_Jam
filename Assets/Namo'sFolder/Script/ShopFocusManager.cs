using UnityEngine;
using TMPro;

// Simplified shop focus - NO camera zoom, just wiggle + Buy/Back/Leave
public class ShopFocusManager : MonoBehaviour
{
    public static ShopFocusManager Instance { get; private set; }

    [Header("UI - Right-Side Info Panel")]
    public GameObject focusUIPanel;
    public TextMeshProUGUI cardInfoText;
    public TextMeshProUGUI cardPriceText;
    [Header("Detailed TMP Fields")]
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI cardRankText;
    public TextMeshProUGUI cardPowerText;
    public TextMeshProUGUI cardSuitsText;
    public TextMeshProUGUI cardEnchantedText;
    public TextMeshProUGUI cardGoldText;

    [Header("Buttons")]
    public UnityEngine.UI.Button purchaseButton;
    public UnityEngine.UI.Button backButton;

    private int focusedIndex = -1;
    private bool isFocused = false;

    public int FocusedIndex => focusedIndex;
    public bool IsFocused => isFocused;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (focusUIPanel != null) focusUIPanel.SetActive(false);
        if (purchaseButton != null) purchaseButton.onClick.AddListener(PurchaseFocusedCard);
        if (backButton != null) backButton.onClick.AddListener(ExitFocus);
    }

    private void OnDestroy()
    {
        if (purchaseButton != null) purchaseButton.onClick.RemoveListener(PurchaseFocusedCard);
        if (backButton != null) backButton.onClick.RemoveListener(ExitFocus);
    }

    private void Update()
    {
        if (isFocused && Input.GetKeyDown(KeyCode.Escape)) ExitFocus();
        // Force Buy to stay clickable while focused (fixes "inactive when click card")
        if (isFocused && purchaseButton != null) purchaseButton.interactable = true;
        // Click outside to exit focus (does not leave shop)
        if (isFocused && Input.GetMouseButtonDown(0) && UnityEngine.EventSystems.EventSystem.current != null && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            var cam = Camera.main;
            if (cam != null)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 50f) && ShopManager.Instance != null && focusedIndex >=0 && focusedIndex < ShopManager.Instance.shopCards.Count)
                {
                    var go = ShopManager.Instance.shopCards[focusedIndex];
                    if (go != null && (hit.collider.gameObject == go || hit.collider.transform.IsChildOf(go.transform))) return;
                }
            }
            ExitFocus();
        }
    }

    public void FocusOnCard(int index)
    {
        if (ShopManager.Instance == null) return;
        if (index < 0 || index >= ShopManager.Instance.shopCards.Count) return;
        var go = ShopManager.Instance.shopCards[index];
        if (go == null) return;

        focusedIndex = index;
        isFocused = true;

        // If focusUIPanel is same as shopUIPanel (your request), keep shop panel active - just update details
        bool sameAsShop = focusUIPanel != null && ShopCameraManager.Instance != null && focusUIPanel == ShopCameraManager.Instance.shopUIPanel;
        if (focusUIPanel != null && !sameAsShop) focusUIPanel.SetActive(true);
        else if (focusUIPanel != null && sameAsShop && ShopCameraManager.Instance != null && !ShopCameraManager.Instance.IsInShop) focusUIPanel.SetActive(true);
        // If same panel, ensure shop panel stays active (don't hide)
        if (sameAsShop && ShopCameraManager.Instance != null && ShopCameraManager.Instance.shopUIPanel != null) ShopCameraManager.Instance.shopUIPanel.SetActive(true);

        UpdateDetailsPanel(index);
        if (SoundManager.Instance != null) SoundManager.Instance.PlayInteractCardSFX();
    }

    private void UpdateDetailsPanel(int index)
    {
        if (ShopManager.Instance == null) return;
        var go = ShopManager.Instance.shopCards[index];
        if (go == null) return;
        var cardObj = go.GetComponent<CardObj>();
        var card = cardObj != null ? cardObj.card : null;
        int price = ShopManager.Instance.GetPrice(index);
        bool canAfford = ShopManager.Instance.CanAfford(index);
        string nameStr = card != null ? card.cardname : $"Card {index}";
        int rank = card != null ? card.Rank : 0;
        int power = rank;
        string suitsStr = "";
        bool isEnchanted = false;
        int suitCount = 0;
        if (card != null && card.suits != null)
        {
            suitCount = card.suits.Count;
            for (int s = 0; s < card.suits.Count; s++) { suitsStr += card.suits[s] != null ? card.suits[s].Name : "?"; if (s < card.suits.Count-1) suitsStr += ", "; }
            if (suitCount > 1)
            {
                isEnchanted = true;
                if (CardManager.instance != null && CardManager.instance.allsuits != null && CardManager.instance.allsuits.Count > 4)
                {
                    isEnchanted = false;
                    for (int i=0;i<card.suits.Count;i++) if (CardManager.instance.allsuits.IndexOf(card.suits[i]) >=4) { isEnchanted=true; break; }
                }
            }
        }
        string enchantedStr = isEnchanted ? "Yes" : "No";
        string priceStr = $"{price} Coins";
        if (cardNameText != null) cardNameText.text = nameStr;
        if (cardRankText != null) cardRankText.text = $"Rank: {rank}";
        if (cardPowerText != null) cardPowerText.text = $"Power: {power}";
        if (cardSuitsText != null) cardSuitsText.text = $"Suits: {suitsStr} ({suitCount})";
        if (cardEnchantedText != null) { cardEnchantedText.text = $"Enchanted: {enchantedStr}"; cardEnchantedText.color = isEnchanted ? new Color(0.6f,0.85f,1f) : Color.white; }
        if (cardGoldText != null) { cardGoldText.text = priceStr; cardGoldText.color = canAfford ? Color.white : Color.red; }
        if (cardInfoText != null) cardInfoText.text = $"{nameStr}\nRank: {rank}  Power: {power}\nSuits: {suitsStr}\nEnchanted: {enchantedStr}";
        if (cardPriceText != null) { cardPriceText.text = priceStr; cardPriceText.color = canAfford ? Color.white : Color.red; }
        // FIX: Keep Buy clickable even if not enough gold (shows red price + shake feedback), was auto-disabled to grey
        if (purchaseButton != null) purchaseButton.interactable = true;
    }

    public void ExitFocus()
    {
        if (!isFocused && focusedIndex==-1 && focusUIPanel != null && !focusUIPanel.activeSelf) return;
        isFocused = false;
        focusedIndex = -1;
        bool sameAsShop = focusUIPanel != null && ShopCameraManager.Instance != null && focusUIPanel == ShopCameraManager.Instance.shopUIPanel;
        // If same panel as shop, don't hide shop when exiting focus (keep shopping)
        if (focusUIPanel != null && !sameAsShop) focusUIPanel.SetActive(false);
        else if (focusUIPanel != null && sameAsShop)
        {
            // Keep shop visible, just clear selection (optional hide details, but keep panel)
            // Don't hide, just keep Buy interactable false until next click
            if (purchaseButton != null) purchaseButton.interactable = false;
        }
        if (SoundManager.Instance != null) SoundManager.Instance.PlayInteractCardSFX();
    }

    public void PurchaseFocusedCard()
    {
        Debug.Log($"[Shop] Buy clicked isFocused={isFocused} idx={focusedIndex} canAfford={(ShopManager.Instance!=null ? ShopManager.Instance.CanAfford(focusedIndex).ToString() : "no mgr")}");
        if (!isFocused || focusedIndex <0) { Debug.LogWarning("[Shop] Buy blocked: not focused"); return; }
        if (ShopManager.Instance == null) return;
        bool ok = ShopManager.Instance.TryBuyCard(focusedIndex);
        if (ok) ExitFocus();
        else
        {
            var shaker = focusUIPanel != null ? focusUIPanel.GetComponent<ObjectShaker>() : null;
            if (shaker==null && focusUIPanel!=null) shaker = focusUIPanel.AddComponent<ObjectShaker>();
            if (shaker!=null) shaker.Shake(0.05f,2f,0.2f);
            var camShake = FindAnyObjectByType<CameraShake>();
            if (camShake!=null) camShake.Shake(0.08f,1.5f,0.15f);
            UpdateDetailsPanel(focusedIndex);
        }
    }

    public void OnBackButton() => ExitFocus();
    public bool IsCardFocused(int index) => isFocused && focusedIndex==index;
}
