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
    public int CoinReward;
    public CardManager.position pos = CardManager.position.other;
    public CardManager.position Pos { get { return pos; } set { pos = value; } }
    [SerializeField] float speed = 0.2f;

    public MeshRenderer MR;
    public Texture M;
    public TextMeshPro PowerText;
    public TextMeshPro HealthText;
    public TextMeshPro NameText;
    public MeshRenderer[] SuitsIcon;

    public bool selectable = true;
    public bool canmove = true;
    void Start()
    {
        BelongR = transform.rotation.eulerAngles;
        MR = GetComponent<MeshRenderer>();
        ResetPos();
        for (int i = 0; i < 6; i++)
        {
            if (i < card.suits.Count)
            {
                SuitsIcon[i].gameObject.SetActive(true);
                SuitsIcon[i].materials[0].SetTexture("_mt", card.suits[i].icon);
                SuitTeller st = SuitsIcon[i].gameObject.GetComponent<SuitTeller>();
                if (st == null)
                {
                    continue;
                }
                st.ThisSuit = card.suits[i];
                st.Des = st.ThisSuit.DescriptionForEnemy;
            }
            else
            {
                SuitsIcon[i].gameObject.SetActive(false);
            }
        }
    }
    void Update()
    {
        ResetPos();
    }
    
    public virtual void ResetPos()
    {
        PowerText.text = card.Power.ToString();
        HealthText.text = card.Health.ToString();
        NameText.text = card.cardname;
        PowerText.text = EncounterManager.instance.CalculateEnemyDmg(card.Power).ToString();
        HealthText.text = card.Health.ToString();

        if (Belong == null || !canmove) return;
        transform.position = Vector3.Lerp(transform.position, Belong, speed);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(BelongR), speed * 5);
    }
    public virtual void MouseEnter()
    {
        string suitss = "Suits:\n";
        foreach (Suit s in card.suits)
        {
            suitss += $"{s.Name}, ";
        }
        Indicator.instance.Move(transform, card.cardname, suitss, Color.red);
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
