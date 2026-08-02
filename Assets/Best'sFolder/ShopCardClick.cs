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
        if (ShopCameraManager.Instance != null && ShopCameraManager.Instance.IsInShop())
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