using UnityEngine;
[CreateAssetMenu(fileName = "Diamond", menuName = "Card/Suits/Diamond")]
public class Diamond : Suit
{
    public override void Enable()
    {
        EventBus<CardAttackerEvent>.Register(Hire);
    }
    public override void Disable()
    {
        EventBus<CardAttackerEvent>.Unregister(Hire);
    }
    public override void EnableForEnemy()
    {
        Disable();
    }
    public override void DisableForEnemy()
    {
        Enable();
    }
    public void Hire(CardAttackerEvent payload)
    {
        if (!payload.Card.suits.Contains(this)) return;

        int num = payload.Card.Rank;
        if (HandOrganizer.Instance.cardsInHand.Count + num > HandOrganizer.Instance.Maxhand)
        {
            num = HandOrganizer.Instance.Maxhand - HandOrganizer.Instance.cardsInHand.Count;
        }
        for (int i = 0; i < num; i++)
        {
            if (Tavern.instance.card.Count > 0)
            {
                CardObj cobj = Tavern.instance.card[Tavern.instance.card.Count - 1];

                if (HandOrganizer.Instance.cardsInHand.Count <= HandOrganizer.Instance.Maxhand)
                {
                    Tavern.instance.RemoveCardFromDeck(cobj);
                }

                HandOrganizer.Instance.AddCardToHand(cobj);
            }
        }
    }
}
