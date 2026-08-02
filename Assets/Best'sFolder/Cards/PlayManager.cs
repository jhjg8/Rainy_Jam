using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
public struct CardMoveToTavern
{
    public Card Card;
}

public struct CardMoveOutOfTavern
{
    public Card Card;
}
public struct CardMoveToHand
{
    public Card Card;
}

public struct CardMoveOutOfHand
{
    public Card Card;
}
public struct CardMoveToGraveyard
{
    public Card Card;
}

public struct CardMoveOutOfGraveyard
{
    public Card Card;
}
public struct CardMoveToField
{
    public Card Card;
}

public struct CardMoveOutOfField
{
    public Card Card;
}
public class PlayManager : MonoBehaviour
{
    public static PlayManager instance;
    public bool Isdefended = true;
    [SerializeField] private CameraShake cs;
    private void Awake()
    {
        instance = this;
    }
    public void DefendButtonPressed()
    {
        if (Isdefended) return;
        Isdefended = true;
        List<CardObj> Defender = new List<CardObj>();
        if (HandOrganizer.Instance.selectedCards.Count != 0)
        {
            Defender = new List<CardObj>(HandOrganizer.Instance.selectedCards);
        }
        foreach (CardObj obj in Defender)
        {
            HandOrganizer.Instance.RemoveCardFromHand(obj);
        }
        int DefencePower = 0;
            DisplayField.Instance.DisplayAndProcess(
            Defender,
            (card) =>
            {
                DefencePower += card.card.CalculateDefendpoint(card.card.Rank);
                EventBus<CardDefenderEvent>.Raise(new CardDefenderEvent { Card = card.card, Enemy = EncounterManager.instance.CurrentEnemy });
                EncounterManager.instance.Defendpoint += card.card.CalculateDefendpoint(card.card.Rank);
                EncounterManager.instance.UpdateUI(true);
                cs.Shake(0.1f,1f,0.1f);
            },
            (completedCards) =>
            {
                if (DefencePower < EncounterManager.instance.CalculateEnemyDmg(EncounterManager.instance.CurrentEnemy.card.Power))
                {
                    Debug.Log("Player lost the encounter!");
                }
                EncounterManager.instance.Defendpoint = 0;
                MoveToGraveYard(completedCards);
            });
        
    }
    public async void MoveToGraveYard(List<CardObj> cardo)
    {
        foreach (CardObj obj in cardo)
        {
            Graveyard.instance.AddCardToDeck(obj);
            await Task.Delay(200);
        }
    }
    public async void Drawcard(int num)
    {
        if (num > Tavern.instance.card.Count) num = Tavern.instance.card.Count;
        for (int i = 0; i < num; i++)
        {
            if (HandOrganizer.Instance.cardsInHand.Count <= HandOrganizer.Instance.Maxhand )
            {
                CardObj card = Tavern.instance.card[Tavern.instance.card.Count-1];
                Tavern.instance.RemoveCardFromDeck(card);
                HandOrganizer.Instance.AddCardToHand(card);
                await Task.Delay(200);
            }
            else
            {
                return;
            }
        }
    }
    public async void Healcard(int num)
    {
        if (num > Graveyard.instance.card.Count) num = Graveyard.instance.card.Count;
        for (int i = 0; i < num; i++)
        {
                CardObj card = Graveyard.instance.card[Graveyard.instance.card.Count - 1];
                Graveyard.instance.RemoveCardFromDeck(card); 
                Tavern.instance.AddCardToDeck(card);
                await Task.Delay(200);
        }
    }
    public void Playcard(CardObj c)
    {
        HandOrganizer.Instance.RemoveCardFromHand(c);
        List<CardObj> ca = new List<CardObj> { c };
        DisplayField.Instance.DisplayAndProcess(ca, 
            (card) =>
        {

            int dmg = EncounterManager.instance.CalculatePlayerdmg(card.card.Rank);
            EncounterManager.instance.DamagePoint += dmg;
            EncounterManager.instance.UpdateUI(false);
            cs.ShakeDefault();
        },
            (completedCards) =>
            {
                Field.Instance.AddCardToField(c);
                EncounterManager.instance.DamagePoint = 0;
            });

    }
    public void AccatkButtonPressed()
    {
        if (!Isdefended) return;

        if (HandOrganizer.Instance.selectedCards.Count > 1) return;

        Isdefended = false;
        CardObj card = HandOrganizer.Instance.selectedCards[0];
        Playcard(card);
        EventBus<CardAttackerEvent>.Raise(new CardAttackerEvent { Card = card.card, Enemy = EncounterManager.instance.CurrentEnemy });
        EncounterManager.instance.CurrentEnemy.card.TakeDamage(
            EncounterManager.instance.CalculatePlayerdmg(card.card.Rank));
    }
}
