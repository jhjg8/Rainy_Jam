using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // Added for Task.Delay
using UnityEngine;

public class EnemyCard : MonoBehaviour
{
    public EnemyCardObject CardObj;
    public int Power = 0;
    public int Health = 10;
    public string cardname = "";

    public CardObj Reward;

    private Material m;

    public List<Suit> suits = new List<Suit>();

    public void Start()
    {
        m = GetComponent<MeshRenderer>().materials[0];
        foreach (var suit in suits)
        {
            suit.EnableForEnemy();
        }
        EventBus<CardAttackerEvent>.Register(Getatkcard);
    }

    public void Addsuits()
    {
        suits = GetComponents<Suit>().ToList();
    }

    public int CalculateDamage(int dmg)
    {
        if (EncounterManager.instance.CalculateEnemyDamage == null) return dmg;
        int newvalue = dmg;
        foreach (Func<int, int> handler in EncounterManager.instance.CalculateEnemyDamage.GetInvocationList())
        {
            newvalue = handler.Invoke(newvalue);
        }
        return newvalue;
    }

    public void TakeDamage(int dmg)
    {
        Health -= dmg;
        if (Health <= 0)
        {
            Died();
        }
        CardObj.ResetPos();
    }

    public void Died()
    {
        EncounterManager.instance.Endfight();
        foreach (Suit suit in suits)
        {
            suit.DisableForEnemy();
        }

        int ran = EncounterManager.instance.DificaultyLevel + UnityEngine.Random.Range(-5, 4) - suits.Count;

        // FIX 1: Spawn the card exactly where the enemy is (transform.position)
        var card = CardManager.instance.createObj(ran, transform.position, suits);
        Reward = card.GetComponent<CardObj>();

        PlayManager.instance.Isdefended = true;
        EventBus<EnemyDiedEvent>.Raise(new EnemyDiedEvent { Card = Reward.card, Enemy = CardObj });

        List<CardObj> cardsOnField = new List<CardObj>(Field.Instance.cardsOnField);
        foreach (var c in cardsOnField)
        {
            Field.Instance.RemoveCardFromField(c);
            Graveyard.instance.AddCardToDeck(c);
        }

        // FIX 2: Trigger a delayed method to send the reward to the graveyard
        ProcessRewardCard(Reward);

        DiedDramaticly();
        EventBus<CardAttackerEvent>.Unregister(Getatkcard);
    }

    // NEW: Async method to delay the Graveyard addition
    private async void ProcessRewardCard(CardObj rewardCard)
    {
        // Wait 1.5 seconds so the player can see the reward pop out of the enemy
        await Task.Delay(1500);

        if (rewardCard != null)
        {
            Graveyard.instance.AddCardToDeck(rewardCard);
        }
    }

    void Getatkcard(CardAttackerEvent CAE)
    {
        Reward = CAE.Card.CardObj;
    }

    void DiedDramaticly()
    {
        m.DOFloat(0f, "_da", 5f)
            .SetEase(Ease.OutQuad) // Makes the dissolve look smooth
            .OnComplete(() =>
            {
                Destroy(gameObject);
            });
    }
}