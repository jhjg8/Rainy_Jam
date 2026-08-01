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

    public int CalculateEnemyDmg(int dmg)
    {
        if (CalculateDef == null) return dmg;
        int newvalue = dmg;
        foreach (Func<int, int> handler in CalculateDef.GetInvocationList())
        {
            newvalue = handler.Invoke(newvalue);
        }
        return newvalue;
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