using UnityEngine;

public class CardManager : MonoBehaviour
{
    public enum position{tavern, graveyard, hand, field};
    public static CardManager instance;
    private void Awake()
    {
        instance = this;
    }

}
