using UnityEngine;
using System.Collections.Generic;
public class Decker : MonoBehaviour
{
    public List<CardObj> card = new List<CardObj>();
    [SerializeField]public float spacing = 0.001f;
    [SerializeField]public Transform Deck;
    public virtual void AddCardToDeck(CardObj Co)
    {
        if (!card.Contains(Co))
        {
            card.Add(Co);
        }
        OrganizeDeck();
    }
    public virtual void RemoveCardFromDeck(CardObj Co)
    {
        if (card.Contains(Co))
        {
            card.Remove(Co);
        }
        OrganizeDeck();
    }
    public virtual void OrganizeDeck()
    {
        int i = 0;
        foreach (CardObj Co in card)
        {
            if (Co != null)
            {
                Co.Belong = Deck.position + new Vector3(0, i * 0.01f, i * spacing);
                i++;
                if (i == card.Count)
                {
                    Co.selectable = true;
                }
                else
                {
                    Co.selectable = false;
                }
                
                if (Co.canmove)
                {
                    Co.ResetPos();
                }
            }
        }
    }
    public void Shuffle()
    {
        card.Shuffle();
    }
}
