using UnityEngine;

public class Tavern : Decker
{
    public override void AddCardToDeck(CardObj Co)
    {
        if (!card.Contains(Co))
        {
            Co.pos = CardManager.position.tavern;
            card.Add(Co);
        }
    }
}
