using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
public class CardObj : MonoBehaviour , IMouseInteractAble
{
    public Card card;
    public Vector3 Belong;
    public Vector3 BelongR;
    public CardManager.position pos = CardManager.position.other;
    public CardManager.position Pos { get { return pos; } set { pos = value; } }
    [SerializeField] float speed = 0.2f;

    public Vector3[] SuitPlacement;
    public MeshRenderer MR;
    public Material M;
    public TextMeshPro RankText;
    public TextMeshPro NameText;

    public bool selectable = true;
    public bool isbacked = true;
    public bool canmove = true;
    void Start()
    {
        Belong = transform.position;
        BelongR = transform.rotation.eulerAngles;
        MR = GetComponent<MeshRenderer>();
        ResetPos();
    }

    public virtual void ResetPos()
    {
        RankText.text = card.Rank.ToString();
        NameText.text = card.cardname;

        if (Belong == null || !canmove) return;
        transform.DOKill();
        transform.DOMove(Belong, speed).SetEase(Ease.OutSine);
        transform.DORotate(BelongR, speed).SetEase(Ease.OutSine);
    }
    public virtual void MouseEnter()
    {
        if (isbacked) return;
        string suitss = "Suits:\n";
        foreach (Suit s in card.suits)
        {
            suitss += $"{s.Name}, ";
        }
        Indicator.instance.Move(transform, card.cardname, suitss);
    }

    public virtual void MouseExit()
    {
        if (isbacked) return;
        Indicator.instance.cancle();
    }

    public virtual void MouseClick()
    {
        if (!selectable) return;
        PlayerDrag.Instance.StartDrag(transform);
        canmove = false;
    }

    public virtual void MouseUp()
    {
        if (!selectable) return;
        PlayerDrag.Instance.StopDrag();
        canmove = true;
        ResetPos() ;
    }
    public void Show()
    {
        MR.materials[0] = CardManager.instance.Back;
        isbacked = false;
    }
    public void Hide()
    {
        GetComponent<MeshRenderer>().materials[0] = M;
        isbacked = true;
    }

}
