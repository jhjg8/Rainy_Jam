using UnityEngine;

public class Card3DInteract : MonoBehaviour
{
    [Header("Card Info")]
    public string cardName = "Card";
    [TextArea] public string cardDescription = "รายละเอียดการ์ด";
    public int price = 10;
    public int cardIndex; // ใบที่ 1 = 0, ใบที่ 2 = 1, ใบที่ 3 = 2

    [Header("Highlight Effects")]
    public float liftAmount = 0.2f;
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void OnMouseEnter()
    {
        transform.position = startPos + new Vector3(0, liftAmount, 0);
    }

    private void OnMouseExit()
    {
        transform.position = startPos;
    }

    private void OnMouseDown()
    {
        OnMouseExit();
        if (ShopFocusManager.Instance != null)
        {
            ShopFocusManager.Instance.FocusOnCard(cardIndex);
        }
    }
}