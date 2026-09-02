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

    public override void EnableForEnemy()
    {
        Disable();
    }

    public override void DisableForEnemy()
    {
        Enable();
    }

    public void Heal(CardAttackerEvent payload)
    {
        // 1. Only run if the attacking card is a Heart
        if (!payload.Card.suits.Contains(this)) return;

        // 2. Shuffle the graveyard first so healing/reviving pulls random cards
        Graveyard.instance.Shuffle();

        // 3. Determine how many cards to recover based on the card's rank
        int numToHeal = payload.Card.Rank;

        // 4. Use the async Healcard method you already built in PlayManager!
        PlayManager.instance.Healcard(numToHeal);
    }
}