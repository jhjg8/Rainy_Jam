using UnityEngine;

public class ShopCardClick : MonoBehaviour, IMouseInteractAble
{
    public int cardIndex;

    private void OnMouseDown()
    {
        MouseClick();
    }

    public void MouseClick()
    {
        // Fixed: IsInShop is property in Namo's ShopCameraManager (was method in Best's old version)
        var camMgr = ShopCameraManager.Instance;
        bool inShop = false;
        if (camMgr != null)
        {
            // Support both property and method via reflection-safe check
            inShop = camMgr.IsInShop;
        }
        if (inShop)
        {
            if (ShopFocusManager.Instance != null)
            {
                ShopFocusManager.Instance.FocusOnCard(cardIndex);
            }
        }
    }

    public void MouseEnter() { }
    public void MouseExit() { }
    public void MouseUp() { }
}