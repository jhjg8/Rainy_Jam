using UnityEngine;

public class Tavern : Decker
{
    public static Tavern instance;
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
            Co.pos = CardManager.position.tavern;
            card.Add(Co);
        }
    }
}
