using UnityEngine;

public class Graveyard : Decker
{
    public override void AddCardToDeck(CardObj Co)
    {
        if (!card.Contains(Co))
        {
            Co.pos = CardManager.position.graveyard;
            card.Add(Co);
        }
    }
}
