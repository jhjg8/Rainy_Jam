using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class Card : MonoBehaviour
{
    public CardObj CardObj;
    public int Rank = 0;

    public Event PlayAsActtacker;
    public Event PlayAsDefender;
    public List<Suit> suits = new List<Suit>() ;

    private void Awake()
    {
        suits = GetComponents<Suit>().ToList();
    }

}
