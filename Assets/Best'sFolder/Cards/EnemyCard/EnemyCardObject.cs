using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
public class EnemyCardObject : MonoBehaviour, IMouseInteractAble
{
    public EnemyCard card;
    public Vector3 Belong;
    public Vector3 BelongR;
    public CardManager.position pos = CardManager.position.other;
    public CardManager.position Pos { get { return pos; } set { pos = value; } }
    [SerializeField] float speed = 0.2f;

    public MeshRenderer MR;
    public Material M;
    public TextMeshPro PowerText;
    public TextMeshPro HealthText;
    public TextMeshPro NameText;
    public MeshRenderer[] SuitsIcon;

    public bool selectable = true;
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
        PowerText.text = card.Power.ToString();
        HealthText.text = card.Health.ToString();
        NameText.text = card.cardname;

        if (Belong == null || !canmove) return;
        transform.DOKill();
        transform.DOMove(Belong, speed).SetEase(Ease.OutSine);
        transform.DORotate(BelongR, speed).SetEase(Ease.OutSine);
    }
    public virtual void MouseEnter()
    {
        string suitss = "Suits:\n";
        foreach (Suit s in card.suits)
        {
            suitss += $"{s.Name}, ";
        }
        Indicator.instance.Move(transform, card.cardname, suitss, false);
    }

    public virtual void MouseExit()
    {
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
        ResetPos();
    }
}
