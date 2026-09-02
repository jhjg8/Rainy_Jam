using UnityEngine;

public class Tavern : Decker
{
    public static Tavern instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    public override void AddCardToDeck(CardObj Co)
    {
        if (!card.Contains(Co))
        {
            Co.pos = CardManager.position.tavern;
            card.Add(Co);
        }
        OrganizeDeck();
        Co.Belong = Deck.position;
        Co.ResetPos();
        EventBus<CardMoveToTavern>.Raise(new CardMoveToTavern { Card = Co.card });
        Co.Hide();
    }
    public override void RemoveCardFromDeck(CardObj Co)
    {
        if (card.Contains(Co))
        {
            card.Remove(Co);
        }
        OrganizeDeck();
        EventBus<CardMoveOutOfTavern>.Raise(new CardMoveOutOfTavern { Card = Co.card });
    }
}
