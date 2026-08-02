using UnityEngine;
using System.Threading.Tasks;
using JetBrains.Annotations;
[CreateAssetMenu(fileName = "Dodge", menuName = "Card/Suits/Dodge")]
public class Dodge : Suit
{
    public override void Enable()
    {
        EventBus<CardAttackerEvent>.Register(dog);
    }
    public override void Disable()
    {
        EventBus<CardAttackerEvent>.Unregister(dog);
    }
    public override void EnableForEnemy()
    {
        Disable();
        EventBus<CardAttackerEvent>.Register(dogforenemy);
    }
    public override void DisableForEnemy()
    {
        Enable();
        EventBus<CardAttackerEvent>.Unregister(dogforenemy);
    }
    public void dog(CardAttackerEvent payload)
    {
        if (!payload.Card.suits.Contains(this)) return;
        Graveyard.instance.Shuffle();
        
        PlayManager.instance.Isdefended = true;
    }
    public async void dogforenemy(CardAttackerEvent payload)
    {
        if (!payload.Card.suits.Contains(this)) return;

        int iRandom = Random.Range(0, 4);
        if (iRandom == 0)
        {
            EncounterManager.instance.CalculatePlayerDamage += d;
            
        }
        await Awaitable.WaitForSecondsAsync(0.5f);

        EncounterManager.instance.CalculatePlayerDamage -= d;
    }
    public int d(int value) => value = 0 ;
}
