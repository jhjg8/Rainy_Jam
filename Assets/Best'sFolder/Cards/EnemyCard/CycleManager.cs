using UnityEngine;

public class CycleManager : MonoBehaviour
{
    public static CycleManager Instance;
    public enum Phase { BuyPhase , FightPhase};
    public Phase CurrentPhase;
    public int Coins;

    private void Awake()
    {
        Instance = this;
    }

    public void ChangePhase()
    {
        if (CurrentPhase == Phase.BuyPhase)
        {
            CurrentPhase = Phase.FightPhase;
            ShopCameraManager.Instance.GoToGame();
            
            // Spawn next enemy
            GameObject Go = EncounterManager.instance.CreateEnemyCard(EncounterManager.instance.enemypos.position);
            EncounterManager.instance.StartFight(Go.GetComponent<EnemyCardObject>());
        }
        else if (CurrentPhase == Phase.FightPhase)
        {
            CurrentPhase = Phase.BuyPhase;
            ShopCameraManager.Instance.GoToShop();
        }
    }
    
    private void Start()
    {
        CurrentPhase = Phase.FightPhase;
        GameObject Go = EncounterManager.instance.CreateEnemyCard(EncounterManager.instance.enemypos.position);
        EncounterManager.instance.StartFight(Go.GetComponent<EnemyCardObject>());
    }
}
