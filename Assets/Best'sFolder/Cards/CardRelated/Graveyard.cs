using UnityEngine;

public class Graveyard : Decker
{
    public static Graveyard instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    public override void AddCardToDeck(CardObj Co)
    {
        if (!card.Contains(Co))
        {
            Co.pos = CardManager.position.graveyard;
            card.Add(Co);
        }
    }
}
