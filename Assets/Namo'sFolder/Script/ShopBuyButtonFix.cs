using UnityEngine;
using UnityEngine.UI;

// Attach to BTN_Buy — forces it stay clickable and buys even if focus was lost
[RequireComponent(typeof(Button))]
public class ShopBuyButtonFix : MonoBehaviour
{
    private Button btn;
    void Awake()
    {
        btn = GetComponent<Button>();
        btn.interactable = true;
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(Buy);
    }
    void OnEnable() { if (btn != null) btn.interactable = true; }
    void Update()
    {
        if (btn != null) btn.interactable = true; // never grey
        if (ShopFocusManager.Instance != null && ShopFocusManager.Instance.IsFocused && ShopFocusManager.Instance.focusUIPanel != null && !ShopFocusManager.Instance.focusUIPanel.activeSelf)
            ShopFocusManager.Instance.focusUIPanel.SetActive(true);
    }
    public void Buy()
    {
        Debug.Log($"[ShopBuyFix] Buy pressed focused={ShopFocusManager.Instance?.IsFocused} idx={ShopFocusManager.Instance?.FocusedIndex} shopNull={ShopManager.Instance==null}");
        if (ShopFocusManager.Instance != null && ShopFocusManager.Instance.IsFocused)
        {
            ShopFocusManager.Instance.PurchaseFocusedCard();
            return;
        }
        // Fallback: if not focused (click without selecting), buy first available shop card
        if (ShopManager.Instance != null)
        {
            for (int i=0;i<ShopManager.Instance.shopCards.Count;i++) if (ShopManager.Instance.shopCards[i]!=null) { Debug.Log($"[ShopBuyFix] Fallback buy idx {i}"); ShopManager.Instance.TryBuyCard(i); break; }
        }
    }
}
