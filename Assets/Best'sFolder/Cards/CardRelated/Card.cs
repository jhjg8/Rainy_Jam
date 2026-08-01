using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Card : MonoBehaviour
{
    public CardObj CardObj;
    public int Rank = 0;
    public string cardname = "";

    public List<Suit> suits = new List<Suit>();
    public Func<int, int> CalculateDef;

    public void Addsuits()
    {
        suits = GetComponents<Suit>().ToList();
    }


    // --- Event Bus Triggers ---

    public void TriggerPlayAsAttacker()
    {
        // Broadcasts to the entire game that THIS card attacked
        EventBus<CardAttackerEvent>.Raise(new CardAttackerEvent { Card = this,Enemy = EncounterManager.instance.CurrentEnemy }); 
    }

    public void TriggerPlayAsDefender()
    {
        // Broadcasts to the entire game that THIS card defended
        EventBus<CardDefenderEvent>.Raise(new CardDefenderEvent { Card = this, Enemy = EncounterManager.instance.CurrentEnemy });
    }
}