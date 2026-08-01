using UnityEngine;
[CreateAssetMenu(fileName = "Heart", menuName = "Card/Suits/Heart")]
public class Heart : Suit
{
    public override void Enable()
    {
        EventBus<CardAttackerEvent>.Register(Heal);
    }
    public override void Disable()
    {
        EventBus<CardAttackerEvent>.Unregister(Heal);
    }
    public void Heal(CardAttackerEvent payload)
    {
        if (!payload.Card.suits.Contains(this)) return;
        Graveyard.instance.Shuffle();
        for (int i = 0; i < payload.Card.Rank; i++)
        {
            if (Graveyard.instance.card.Count > 0)
            {
                CardObj cobj = Graveyard.instance.card[Graveyard.instance.card.Count - 1];

                Graveyard.instance.RemoveCardFromDeck(cobj);

                Tavern.instance.AddCardToDeck(cobj);
            }
        }
    }
}
