using UnityEngine;

public class PlayManager : MonoBehaviour
{
    public static PlayManager instance;
    private void Awake()
    {
        instance = this;
    }
}
