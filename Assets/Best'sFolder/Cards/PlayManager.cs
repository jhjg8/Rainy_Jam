using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    private void Awake()
    {
        instance = this;
    }
    public void PlayAsDefend(List<CardObj> defend)
    {
        foreach (CardObj obj in defend)
        {
            obj.card.TriggerPlayAsDefender();
        }

    }
    public void DefendButtonPressed()
    {
        List<CardObj> Defender = new List<CardObj>();
        if (HandOrganizer.Instance.selectedCards.Count != 0)
        {
            Defender = HandOrganizer.Instance.selectedCards;
        }
        foreach (CardObj obj in Defender)
        {
            HandOrganizer.Instance.selectedCards.Remove(obj);
        }
        DisplayField.Instance.DisplayAndProcess(
            Defender,
            (card) =>
            {
                card.card.TriggerPlayAsDefender();
                EncounterManager.instance.Defendpoint += card.card.CalculateDefendpoint(card.card.Rank);

            },
            (completedCards) =>
            {
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
        if (num <= Tavern.instance.card.Count) num = Tavern.instance.card.Count;
        for (int i = 0; i < num; i++)
        {
            if (HandOrganizer.Instance.cardsInHand.Count <= HandOrganizer.Instance.Maxhand)
            {
                CardObj card = Tavern.instance.card[Tavern.instance.card.Count];
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
        if (num <= Graveyard.instance.card.Count) num = Graveyard.instance.card.Count;
        for (int i = 0; i < num; i++)
        {
                CardObj card = Graveyard.instance.card[Graveyard.instance.card.Count];
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
            card.card.TriggerPlayAsDefender();
            EncounterManager.instance.CalculatePlayerDamage(card.card.Rank);

        },
            (completedCards) =>
            {
                Field.Instance.AddCardToField(c);
            });

    }
    public void AccatkButtonPressed()
    {
        if (!Isdefended) return;

        if (HandOrganizer.Instance.selectedCards.Count > 1) return;

        CardObj card = HandOrganizer.Instance.selectedCards[0];
        Playcard(card);
        EventBus<CardAttackerEvent>.Raise(new CardAttackerEvent { Card = card.card , Enemy = EncounterManager.instance.CurrentEnemy });
    }
}
