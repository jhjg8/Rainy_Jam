using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        var card = CardManager.instance.createObj(ran, transform.position, suits);
        Graveyard.instance.AddCardToDeck(card.GetComponent<CardObj>());
        Reward = card.GetComponent<CardObj>();
        PlayManager.instance.Isdefended = true;
        EventBus<EnemyDiedEvent>.Raise(new EnemyDiedEvent { Card = Reward.card, Enemy = CardObj });
        DiedDramaticly();
        EventBus<CardAttackerEvent>.Unregister(Getatkcard);
    }
    void Getatkcard(CardAttackerEvent CAE)
    {
        Reward = CAE.Card.CardObj;
    }
    void DiedDramaticly()
    {
        m.DOFloat(1f, "_da", 1.5f)
            .SetEase(Ease.OutQuad) // Makes the dissolve look smooth
            .OnComplete(() =>
            {
                Destroy(gameObject);
            });
    }
}
