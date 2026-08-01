using UnityEngine;

public class KickStart : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EncounterManager.instance.StartFight();   
    }
}
