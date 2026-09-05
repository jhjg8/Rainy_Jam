using UnityEngine;
using System.Collections.Generic;
public class KickStart : MonoBehaviour
{
  
    void Start()
    {
        List<CardObj> cards = new List<CardObj>();
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j <= 5; j++)
            {
                List<Suit> s = new List<Suit> { CardManager.instance.allsuits[i] };
                GameObject card = CardManager.instance.createObj(j + 5, new Vector3(0, 0, 0), s);
                cards.Add(card.GetComponent<CardObj>());
            }
        }
        cards.Shuffle();
        foreach (CardObj card in cards)
        {
            Tavern.instance.AddCardToDeck(card);
        }
        PlayManager.instance.Drawcard(HandOrganizer.Instance.Maxhand);
        // FIX: Removed duplicate enemy spawn — CycleManager.Start already spawns first enemy.
        // If you need KickStart to spawn, disable CycleManager's Start spawn instead.
    }
}
