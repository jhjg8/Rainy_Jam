using UnityEngine;
using DG.Tweening;
using TMPro;
public class CardObj : MonoBehaviour , IMouseInteractAble
{
    public Card card;
    public Vector3 Belong;
    private CardManager.position pos = CardManager.position.other;
    public CardManager.position Pos { get { return pos; } set { pos = value; } }
    [SerializeField] float speed = 2f;

    public Vector3[] SuitPlacement;
    public MeshRenderer MR;
    public TextMeshPro RankText;
    public TextMeshPro NameText;

    public bool canmove = true;
    void Start()
    {
        Belong = transform.position;
        ResetPos();
    }

    public void ResetPos()
    {
        RankText.text = card.Rank.ToString();
        NameText.text = card.cardname;

        if (Belong == null || !canmove) return;
        transform.DOKill();
        transform.DOMove(Belong, speed).SetEase(Ease.OutSine);
    }
    public void MouseEnter()
    {
        string suitss = "Suits:\n";
        foreach (Suit s in card.suits)
        {
            suitss += $"{s.Name}, ";
        }
        Indicator.instance.Move(transform, card.cardname, suitss);
    }

    public void MouseExit()
    {
        Indicator.instance.cancle();
    }

    public void MouseClick()
    {
        PlayerDrag.Instance.StartDrag(transform);
        canmove = false;
    }

    public void MouseUp()
    {
        PlayerDrag.Instance.StopDrag();
        canmove = true;
    }

}
