using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
public class EnemyCard : MonoBehaviour
{
    public EnemyCardObject CardObj;
    public int Power = 0;
    public int Health = 10;
    public string cardname = "";

    public List<Suit> suits = new List<Suit>();
    public void Start()
    {
        foreach (var suit in suits)
        {
            suit.EnableForEnemy();
        }
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
            EncounterManager.instance.Endfight();
            foreach(Suit suit in suits)
            {
                suit.DisableForEnemy();
            }
        }
    }
}
