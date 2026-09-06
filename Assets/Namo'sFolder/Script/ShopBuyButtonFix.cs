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
        Debug.Log($"[ShopBuyFix] Buy pressed focused={ShopFocusManager.Instance?.IsFocused} idx={ShopFocusManager.Instance?.FocusedIndex}");
        if (ShopFocusManager.Instance != null && ShopFocusManager.Instance.IsFocused)
        {
            ShopFocusManager.Instance.PurchaseFocusedCard();
        }
        else
        {
            Debug.LogWarning("[Shop] Select a card first, then Buy");
            var panel = ShopFocusManager.Instance != null ? ShopFocusManager.Instance.focusUIPanel : null;
            if (panel != null) { var sh = panel.GetComponent<ObjectShaker>(); if (sh==null) sh=panel.AddComponent<ObjectShaker>(); sh.Shake(0.05f,2f,0.2f); }
        }
    }
}
