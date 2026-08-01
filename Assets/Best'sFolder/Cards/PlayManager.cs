using UnityEngine;
using System.Collections.Generic;
public class PlayManager : MonoBehaviour
{
    public static PlayManager instance;
    private void Awake()
    {
        instance = this;
    }
    public void PlayAsDefend(List<CardObj> defend)
    {
        foreach (CardObj obj in defend)
        {
            obj.card.TriggerPlayAsDefender();
        }

    }
}
