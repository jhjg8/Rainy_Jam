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

    public int CalculateDefendpoint(int point)
    {
        if (CalculateDef == null) return point;
        int newvalue = point;
        foreach (Func<int, int> handler in CalculateDef.GetInvocationList())
        {
            newvalue = handler.Invoke(newvalue);
        }
        return newvalue;
    }
    // --- Event Bus Triggers ---

    public void TriggerPlayAsAttacker()
    {
    }

    public void TriggerPlayAsDefender()
    {
    }
}