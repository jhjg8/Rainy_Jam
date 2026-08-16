using UnityEngine;

public class SuitTeller : MonoBehaviour ,IMouseInteractAble
{
    public Suit ThisSuit;
    public string Des = "";
    public void MouseClick()
    {
        GetComponentInParent<CardObj>().MouseClick();
    }

    public virtual void MouseEnter()
    {
        Indicator.instance.Move(transform, ThisSuit.Name, Des, Color.purple);
        CardObj Co = GetComponentInParent<CardObj>();
        if (Co != null)
        {
            Co.suitChoosen = true;
        }
    }

    public virtual void MouseExit()
    {
        Indicator.instance.cancle();
        CardObj Co = GetComponentInParent<CardObj>();
        if (Co != null)
        {
            Co.suitChoosen = false;
        }
    }

    public void MouseUp()
    {
        GetComponentInParent<CardObj>().MouseUp();
    }
}
