using UnityEngine;
using System.Threading.Tasks;
[CreateAssetMenu(fileName = "Doctor", menuName = "Card/Suits/Doctor")]
public class Doctor : Suit
{
    public override void Enable()
    {
        EventBus<CardAttackerEvent>.Register(Unwound);
    }
    public override void Disable()
    {
        EventBus<CardAttackerEvent>.Unregister(Unwound);
    }
    public override void EnableForEnemy()
    {
        Disable();
        EventBus<CardAttackerEvent>.Register(Heal);
    }
    public override void DisableForEnemy()
    {
        Enable();
        EventBus<CardAttackerEvent>.Unregister(Heal);
    }
    public void Unwound(CardAttackerEvent payload)
    {
        if (!payload.Card.suits.Contains(this)) return;
        Graveyard.instance.Shuffle();
        int num = payload.Card.Rank;
        if (HandOrganizer.Instance.cardsInHand.Count > HandOrganizer.Instance.Maxhand)
        {
            num = HandOrganizer.Instance.cardsInHand.Count - HandOrganizer.Instance.Maxhand;
        }
        for (int i = 0; i < num; i++)
        {
            CardObj cobj = Graveyard.instance.card[HandOrganizer.Instance.cardsInHand.Count - 1];
            Graveyard.instance.RemoveCardFromDeck(cobj);
            HandOrganizer.Instance.AddCardToHand(cobj);
        }
    }
    public async void Heal(CardAttackerEvent payload)
    {
        if (!payload.Card.suits.Contains(this)) return;

        await Awaitable.WaitForSecondsAsync(1.5f);
        payload.Enemy.card.Health += payload.Card.Rank;
    }
}
