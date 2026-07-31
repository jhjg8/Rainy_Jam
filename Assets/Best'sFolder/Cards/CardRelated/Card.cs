using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Card : MonoBehaviour
{
    public CardObj CardObj;
    public int Rank = 0;

    public string cardname = "";
    public Event PlayAsActtacker;
    public Event PlayAsDefender;
    public List<Suit> suits = new List<Suit>() ;
    public Func<int, int> Calculate;

    public void Addsuits()
    {
        suits = GetComponents<Suit>().ToList();
    }
    public int CalculateDamage(int dmg)
    {
        if (Calculate == null) return dmg;
        int newvalue = dmg;
        foreach (Func<int, int> handler in Calculate.GetInvocationList())
        {
            newvalue = handler.Invoke(newvalue);
        }
        return newvalue;
    }
}
