using UnityEngine;

public class Suit : MonoBehaviour
{
    public string id = "";
    public string Name = "name";
    public Sprite icon;
    CardObj CO;
    Card C;
    protected virtual void Start()
    {
        CO = GetComponent<CardObj>();
        C = CO.card;
    }
    protected virtual void Update()
    {

    }

    public virtual void Enable()
    {

    }
    public virtual void Disable()
    {

    }
}
