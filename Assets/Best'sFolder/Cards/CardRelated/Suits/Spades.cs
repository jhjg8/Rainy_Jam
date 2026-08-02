using UnityEngine;

[CreateAssetMenu(fileName = "Spades", menuName = "Card/Suits/Spades")]
public class Spades : Suit
{
    // Keep a running total instead of overwriting a single rank
    private int totalDamageReduction = 0;
    private bool isSubscribed = false;

    public override void Enable()
    {
        EventBus<CardAttackerEvent>.Register(Break);

        // Fixed syntax: Added the method to register
        EventBus<EnemyDiedEvent>.Register(Undone);
    }

    public override void Disable()
    {
        EventBus<CardAttackerEvent>.Unregister(Break);
        EventBus<EnemyDiedEvent>.Unregister(Undone);

        // Clean up safely if disabled mid-combat
        if (isSubscribed)
        {
            EncounterManager.instance.CalculateEnemyDamage -= Defend;
            isSubscribed = false;
        }
        totalDamageReduction = 0;
    }

    public override void EnableForEnemy()
    {
        Disable();
    }

    public override void DisableForEnemy()
    {
        Enable();
    }

    public void Break(CardAttackerEvent payload)
    {
        if (payload.Card.suits.Contains(this))
        {
            // Stack the new rank onto the total damage reduction
            totalDamageReduction += payload.Card.Rank;

            // Only add the delegate to the EventBus ONCE
            if (!isSubscribed)
            {
                EncounterManager.instance.CalculateEnemyDamage += Defend;
                isSubscribed = true;
            }
        }
    }

    public void Undone(EnemyDiedEvent payload)
    {
        // When the enemy dies, clear the delegate and reset the stack to 0
        if (isSubscribed)
        {
            EncounterManager.instance.CalculateEnemyDamage -= Defend;
            isSubscribed = false;
        }
        totalDamageReduction = 0;
    }

    public int Defend(int value)
    {
        // Subtract the total stacked value from all Spades played this encounter
        return value - totalDamageReduction;
    }
}