using UnityEngine;
[CreateAssetMenu(fileName = "BigHand", menuName = "Card/Suits/BigHand")]
public class BigHand : Suit
{
    public override void Enable()
    {
        foreach (CardObj card in Tavern.instance.card)
        {
            if (card.card.suits.Contains(this))
            {
                HandOrganizer.Instance.Maxhand += 1;
            }
        }
        EventBus<CardMoveToTavern>.Register(AddHand);
        EventBus<CardMoveOutOfTavern>.Register(RemoveHand);
    }
    public override void Disable()
    {
        foreach (CardObj card in Tavern.instance.card)
        {
            if (card.card.suits.Contains(this))
            {
                HandOrganizer.Instance.Maxhand -= 1;
            }
        }
        EventBus<CardMoveToTavern>.Unregister(AddHand);
        EventBus<CardMoveOutOfTavern>.Unregister(RemoveHand);
    }
    public override void EnableForEnemy()
    {
        Disable();
    }
    public override void DisableForEnemy()
    {
        Enable();
    }

    public void AddHand(CardMoveToTavern payload)
    {
        if (!payload.Card.suits.Contains(this)) return;
        HandOrganizer.Instance.Maxhand += 1;
    }
    public void RemoveHand(CardMoveOutOfTavern payload)
    {
        if (!payload.Card.suits.Contains(this)) return;
        HandOrganizer.Instance.Maxhand -= 1;
    }
}
