using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
public class CardObj : MonoBehaviour, IMouseInteractAble
{
    public Card card;
    public Vector3 Belong;
    public Vector3 BelongR;
    public CardManager.position pos = CardManager.position.other;
    public CardManager.position Pos { get { return pos; } set { pos = value; } }
    [SerializeField] float speed = 10f;

    public MeshRenderer MR;
    public Texture M;
    public TextMeshPro RankText;
    public TextMeshPro NameText;
    public MeshRenderer[] SuitsIcon;

    public bool selectable = true;
    public bool isbacked = false;
    public bool canmove = true;
    public bool suitChoosen = false;
    public bool thisChoosen = false;
    void Start()
    {
        Belong = transform.position;
        BelongR = transform.rotation.eulerAngles;
        if (MR == null) MR = GetComponent<MeshRenderer>();
        ResetPos();
    }
    void Update()
    {
        ResetPos();
    }
    public virtual void ResetPos()
    {
        RankText.text = card.Rank.ToString();
        NameText.text = card.cardname;

        if (Belong == null || !canmove) return;
        transform.DOKill();
        transform.position = Vector3.Lerp(transform.position, Belong, speed);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(BelongR), speed * 5);
    }
    public virtual void MouseEnter()
    {
        if (isbacked) return;
        if (suitChoosen) return;

        StartCoroutine(ME());
    }
    IEnumerator ME()
    {
        yield return new WaitForEndOfFrame();
        if (suitChoosen) yield break;
        if (pos == CardManager.position.graveyard || pos == CardManager.position.tavern) yield break;
        if (card.UseAble == false)
        {
            Indicator.instance.Move(transform, card.cardname, "This card is not usable", Color.blue);
            yield break;
        }
        else
        {
            string suitss = "Suits:\n";
            foreach (Suit s in card.suits)
            {
                suitss += $"{s.Name}, ";
            }
            Indicator.instance.Move(transform, card.cardname, suitss, Color.blue);
            HandOrganizer.Instance.TogglelittleSelectCard(this, true);
        }
    }
    public virtual void MouseExit()
    {
        if (suitChoosen) return;
        if (isbacked) return;
        StartCoroutine(MX());
    }
    IEnumerator MX()
    {
        yield return new WaitForEndOfFrame();
        if (suitChoosen) yield break;
        if (pos == CardManager.position.graveyard || pos == CardManager.position.tavern) yield break;
        Indicator.instance.cancle();
        HandOrganizer.Instance.TogglelittleSelectCard(this, false);
    }
    public virtual void MouseClick()
    {
        if (!selectable) return;
        // Cards in graveyard or tavern cannot be interacted with directly
        if (pos == CardManager.position.graveyard || pos == CardManager.position.tavern) return;

        bool inShop = (CycleManager.Instance != null && CycleManager.Instance.CurrentPhase == CycleManager.Phase.BuyPhase)
                     || (ShopCameraManager.Instance != null && ShopCameraManager.Instance.IsInShop);
        if (inShop)
        {
            if (ShopManager.Instance != null && ShopManager.Instance.shopCards.Contains(gameObject))
            {
                int idx = ShopManager.Instance.shopCards.IndexOf(gameObject);
                if (ShopFocusManager.Instance != null) ShopFocusManager.Instance.FocusOnCard(idx);
                return;
            }
        }

        PlayerDrag.Instance.StartDrag(transform);
        canmove = false;
        if (pos == CardManager.position.hand)
        {
            HandOrganizer.Instance.ToggleSelectCard(this,true);
        }
    }

    public virtual void MouseUp()
    {
        if (!selectable) return;
        if (pos == CardManager.position.graveyard || pos == CardManager.position.tavern) return;
        PlayerDrag.Instance.StopDrag();
        canmove = true;
        HandOrganizer.Instance.ReorderCardByPosition(this);
        ResetPos();
    }
    public void Show()
    {
        MR.materials[0].SetTexture("_mt", M);
        isbacked = false;
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
                st.Des = st.ThisSuit.Description;
            }
            else
            {
                SuitsIcon[i].gameObject.SetActive(false);
            }
        }
        RankText.gameObject.SetActive(true);
        NameText.gameObject.SetActive(true);
    }
    public void Hide()
    {
        MR.materials[0].SetTexture("_mt", CardManager.instance.Back);
        isbacked = true;
        for (int i = 0; i < SuitsIcon.Length; i++)
        {
            SuitsIcon[i].gameObject.SetActive(false);
        }
        RankText.gameObject.SetActive(false);
        NameText.gameObject.SetActive(false);
    }

}
