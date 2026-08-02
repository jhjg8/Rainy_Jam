using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Guardian", menuName = "Card/Suits/Guardian")]
public class Guardian : Suit
{
    public override void Enable()
    {
        EventBus<CardDefenderEvent>.Register(Guard);
    }
    public override void Disable()
    {
        EventBus<CardDefenderEvent>.Unregister(Guard);
    }
    public override void EnableForEnemy()
    {
        Disable();
        EncounterManager.instance.CalculatePlayerDamage += EnhanceGuardForEnemy;
    }
    public override void DisableForEnemy()
    {
        Enable();
        EncounterManager.instance.CalculatePlayerDamage -= EnhanceGuardForEnemy;
    }
    public void Guard(CardDefenderEvent payload)
    {
        if (!payload.Card.suits.Contains(this)) return;

        List<CardObj> displayedCards = DisplayField.Instance.currentDisplayedCards;

        foreach (var card in displayedCards)
        {
            if (card != null)
            {
                // 1. Declare the delegate variable first so it can reference itself
                Func<int, int> oneTimeGuard = null;

                // 2. Define the behavior
                oneTimeGuard = (value) =>
                {
                    // 3. Immediately remove THIS specific function from the card 
                    // so it never triggers again.
                    card.card.CalculateDef -= oneTimeGuard;

                    // 4. Return the buffed value
                    return value + 2;
                };

                // 5. Add it to the card
                card.card.CalculateDef += oneTimeGuard;
            }
        }
    }
    public int EnhanceGuard(int value) => value + 2;
    public int EnhanceGuardForEnemy(int value) => value - (int)((float)value / 1.5f);
}
