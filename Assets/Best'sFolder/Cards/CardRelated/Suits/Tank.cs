using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Tank", menuName = "Card/Suits/Tank")]
public class Tank : Suit
{
    Dictionary<Card, int> used = new Dictionary<Card, int>();
    Dictionary<Card, bool> isusedasdefender = new Dictionary<Card, bool>();
    public override void Enable()
    {
        EventBus<CardMoveToGraveyard>.Register(Reuse);
        EventBus<CardDefenderEvent>.Register(isused);
        EventBus<EnemyDiedEvent>.Register(Re);
    }
    public override void Disable()
    {
        EventBus<CardMoveToGraveyard>.Unregister(Reuse);
        EventBus<CardDefenderEvent>.Unregister(isused);
        EventBus<EnemyDiedEvent>.Register(Re);
    }
    public override void EnableForEnemy()
    {
        Disable();
        EncounterManager.instance.CurrentEnemy.card.Health = (int)((EncounterManager.instance.CurrentEnemy.card.Health) * 1.5f);
    }
    public override void DisableForEnemy()
    {
        Enable();
    }
    public void Reuse(CardMoveToGraveyard payload)
    {
        if (!payload.Card.suits.Contains(this)) return;
        Graveyard.instance.Shuffle();
        
        if (!used.ContainsKey(payload.Card)) used.Add(payload.Card, 2);
        if (used[payload.Card] > 0 && isusedasdefender[payload.Card])
        {
            if (HandOrganizer.Instance.cardsInHand.Count < HandOrganizer.Instance.Maxhand)
            {
                used[payload.Card]--;
                isusedasdefender[payload.Card] = false;
                Graveyard.instance.RemoveCardFromDeck(payload.Card.CardObj);
                HandOrganizer.Instance.AddCardToHand(payload.Card.CardObj);
            }
        }
    }
    public void isused(CardDefenderEvent payload)
    {
        if (isusedasdefender.ContainsKey(payload.Card)) isusedasdefender[payload.Card] = true;
        else isusedasdefender.Add(payload.Card, true);
    }
    public void Re(EnemyDiedEvent payload)
    {
        used.Clear();
        isusedasdefender.Clear();
    }
}
