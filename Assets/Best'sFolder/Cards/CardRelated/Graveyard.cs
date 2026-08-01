using UnityEngine;

public class Graveyard : Decker
{
    public static Graveyard instance;
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
            Co.pos = CardManager.position.graveyard;
            card.Add(Co);
        }
        EventBus<CardMoveToGraveyard>.Raise(new CardMoveToGraveyard { Card = Co.card });
    }
    public override void RemoveCardFromDeck(CardObj Co)
    {
        if (card.Contains(Co))
        {
            card.Remove(Co);
        }
        OrganizeDeck();
        EventBus<CardMoveOutOfGraveyard>.Raise(new CardMoveOutOfGraveyard { Card = Co.card });
    }
}
