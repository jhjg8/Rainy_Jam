using UnityEngine;
[CreateAssetMenu(fileName = "Juggernaut", menuName = "Card/Suits/Juggernaut")]
public class Juggernaut : Suit
{
    public override void Enable()
    {
        EventBus<CardAttackerEvent>.Register(DamageBoost);
    }
    public override void Disable()
    {
        EventBus<CardAttackerEvent>.Unregister(DamageBoost);
    }
    public override void EnableForEnemy()
    {
        Disable();
    }
    public override void DisableForEnemy()
    {
        Enable();
    }
    public void DamageBoost(CardAttackerEvent payload)
    {
        EncounterManager.instance.CalculatePlayerDamage -= BoostDamage;
        if (payload.Card.suits.Contains(this))
        {
            EncounterManager.instance.CalculatePlayerDamage += BoostDamage;
        }
    }
    public int BoostDamage(int value) 
    { 
        int i = 0;
        foreach (CardObj card in HandOrganizer.Instance.cardsInHand)
        {
            i += card.card.Rank;
        }
        return value + i; 
    }
}
