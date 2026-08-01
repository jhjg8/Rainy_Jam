using UnityEngine;

public class CycleManager : MonoBehaviour
{
    public enum Phase { BuyPhase , FightPhase};
    public Phase CurrentPhase;
    public void ChangePhase()
    {
        if (CurrentPhase == Phase.BuyPhase)
        {
            CurrentPhase = Phase.FightPhase;
        }
        else if (CurrentPhase == Phase.FightPhase)
        {
            CurrentPhase = Phase.BuyPhase;
        }
    }
    private void Start()
    {
        CurrentPhase = Phase.FightPhase;
        EncounterManager.instance.StartFight();
    }
}
