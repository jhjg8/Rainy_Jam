using UnityEngine;
[CreateAssetMenu(fileName = "BigHand", menuName = "Card/Suits/BigHand")]
public class BigHand : Suit
{
    public override void Enable()
    {
        EventBus<CardMoveToTavern>.Register(AddHand);
        EventBus<CardMoveOutOfTavern>.Register(RemoveHand);
    }
    public override void Disable()
    {
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
