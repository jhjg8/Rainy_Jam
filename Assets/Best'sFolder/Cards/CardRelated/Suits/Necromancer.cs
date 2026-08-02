using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "Necromancer", menuName = "Card/Suits/Necromancer")]
public class Necromancer : Suit
{
    int cardrank;
    List<Card> Blacklist = new List<Card>();
    public override void Enable()
    {
        EventBus<EnemyDiedEvent>.Register(Revive);
    }
    public override void Disable()
    {
        EventBus<EnemyDiedEvent>.Unregister(Revive);
    }
    public override void EnableForEnemy()
    {
        Disable();
        EventBus<CardAttackerEvent>.Register(Unuse);
    }
    public override void DisableForEnemy()
    {
        Enable();
        EventBus<CardAttackerEvent>.Unregister(Unuse);
        foreach (Card card in Blacklist)
        {
            card.UseAble = true;
        }
        Blacklist.Clear();
    }
    public void Revive(EnemyDiedEvent payload)
    {
        bool hasNecro = false;
        foreach (var c in Field.Instance.cardsOnField)
        {
            if (c.card.suits.Contains(this))
            {
                hasNecro = true;
                break;
            }
        }
        if (!hasNecro) return;

        if (HandOrganizer.Instance.cardsInHand.Count >= HandOrganizer.Instance.Maxhand) return;

        Graveyard.instance.RemoveCardFromDeck(payload.Enemy.card.Reward);
        HandOrganizer.Instance.AddCardToHand(payload.Enemy.card.Reward);
    }
    private void Unuse(CardAttackerEvent payload)
    {
        payload.Card.UseAble = false;
        Blacklist.Add(payload.Card);
    }
}
