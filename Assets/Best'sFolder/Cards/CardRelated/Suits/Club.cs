using UnityEngine;
[CreateAssetMenu(fileName = "Club", menuName = "Card/Suits/Club")]
public class Club : Suit
{
    public override void Enable()
    {
        EventBus<CardAttackerEvent>.Register(Attack);
    }
    public override void Disable()
    {
        EventBus<CardAttackerEvent>.Unregister(Attack);
    }
    public override void EnableForEnemy()
    {
        Disable();
    }
    public override void DisableForEnemy()
    {
        Enable();
    }
    public int Twotimes(int value) =>  value * 2 ;
    
    public void Attack(CardAttackerEvent payload)
    {
        if (!payload.Card.suits.Contains(this)) return;
        payload.Card.Calculate += Twotimes;
    }
}
