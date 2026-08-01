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
                GameObject card = CardManager.instance.createObj(j, new Vector3(0, 0, 0), s);
                cards.Add(card.GetComponent<CardObj>());
            }
        }
        cards.Shuffle();
        foreach (CardObj card in cards)
        {
            Tavern.instance.AddCardToDeck(card);
        }
        PlayManager.instance.Drawcard(HandOrganizer.Instance.Maxhand);
        GameObject go = EncounterManager.instance.CreateEnemyCard(new Vector3(0,0,0));
        EncounterManager.instance.StartFight(go.GetComponent<EnemyCardObject>());   
    }
}
