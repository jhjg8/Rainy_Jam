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
        // Leaving blank for now (see note below)
    }

    public override void DisableForEnemy()
    {
        // Leaving blank for now
    }

    public void Hire(CardAttackerEvent payload)
    {
        // 1. Only run if the attacking card is a Diamond
        if (!payload.Card.suits.Contains(this)) return;

        // 2. Calculate how many cards we can fit
        int numToDraw = payload.Card.Rank;
        int availableSpace = HandOrganizer.Instance.Maxhand - HandOrganizer.Instance.cardsInHand.Count;

        if (numToDraw > availableSpace)
        {
            numToDraw = availableSpace;
        }

        // 3. Just use the fantastic async method you already wrote!
        PlayManager.instance.Drawcard(numToDraw);
    }
}