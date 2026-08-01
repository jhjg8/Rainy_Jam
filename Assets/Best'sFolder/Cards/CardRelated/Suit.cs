using UnityEngine;

public class Suit : ScriptableObject
{
    public string id = "";
    public string Name = "name";
    public Material icon;
    protected CardObj CO;
    protected Card C;
    protected virtual void Update()
    {

    }

    public virtual void Enable()
    {

    }
    public virtual void Disable()
    {

    }
    public virtual void EnableForEnemy()
    {

    }
    public virtual void DisableForEnemy()
    {

    }
}
