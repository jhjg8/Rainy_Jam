using UnityEngine;
[CreateAssetMenu(fileName = "Spades", menuName = "Card/Suits/Spades")]
public class Spades : Suit
{
    int cardrank;
    public override void Enable()
    {
        EventBus<CardAttackerEvent>.Register(Break);
    }
    public override void Disable()
    {
        EventBus<CardAttackerEvent>.Unregister(Break);
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
        if (!payload.Card.suits.Contains(this)) return;
        // The lambda captures 'payload' and executes the math
        cardrank = payload.Card.Rank;
        EncounterManager.instance.CalculateEnemyDamage += Defend;
    }
    public int Defend(int value)
    {
        return value - cardrank;
    }
}
