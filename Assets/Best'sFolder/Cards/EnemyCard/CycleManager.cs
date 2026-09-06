using UnityEngine;

public class CycleManager : MonoBehaviour
{
    public static CycleManager Instance;
    public enum Phase { BuyPhase , FightPhase};
    public Phase CurrentPhase;
    public int Coins = 20; // start with 20 so you can test Buy without winning

    [Header("Shop Button (enable after win)")]
    public GameObject shopButton; // assign Shop button, hidden until boss defeated
    [HideInInspector] public bool shopAvailable = false;

    private void Awake()
    {
        Instance = this;
        if (shopButton != null) shopButton.SetActive(false);
    }

    // Called by EncounterManager when boss dies — does NOT auto-enter shop, just enables button after fight cleanup
    public void OnBossDefeated()
    {
        // FIX: Wait for DisplayField/Defendpoint cleanup (was causing "enemydamage reset -> eliminated" if clicked instantly)
        StartCoroutine(EnableShopAfterDelay(0.6f));
    }

    private System.Collections.IEnumerator EnableShopAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Ensure fight values reset before shop
        if (EncounterManager.instance != null)
        {
            EncounterManager.instance.Defendpoint = 0;
            EncounterManager.instance.DamagePoint = 0;
        }
        shopAvailable = true;
        if (shopButton != null) shopButton.SetActive(true);
        Debug.Log("[CycleManager] Boss defeated — Shop button enabled, press to enter shop");
    }

    public void EnterShop()
    {
        if (!shopAvailable) return;
        CurrentPhase = Phase.BuyPhase;
        ShopCameraManager.Instance.GoToShop();
        if (shopButton != null) shopButton.SetActive(false);
    }

    public void ExitShopAndStartNextRound()
    {
        CurrentPhase = Phase.FightPhase;
        shopAvailable = false;
        ShopCameraManager.Instance.GoToGame();
        // Auto-start next round after leaving shop
        GameObject Go = EncounterManager.instance.CreateEnemyCard(EncounterManager.instance.enemypos.position);
        EncounterManager.instance.StartFight(Go.GetComponent<EnemyCardObject>());
        if (shopButton != null) shopButton.SetActive(false);
    }

    public void ChangePhase()
    {
        // Legacy toggle — now routes to manual flow for backward compat
        if (CurrentPhase == Phase.BuyPhase)
        {
            ExitShopAndStartNextRound();
        }
        else if (CurrentPhase == Phase.FightPhase)
        {
            // If shop is available, enter it; otherwise just toggle (for test)
            if (shopAvailable) EnterShop();
            else
            {
                CurrentPhase = Phase.BuyPhase;
                ShopCameraManager.Instance.GoToShop();
            }
        }
    }
    
    private void Start()
    {
        CurrentPhase = Phase.FightPhase;
        GameObject Go = EncounterManager.instance.CreateEnemyCard(EncounterManager.instance.enemypos.position);
        EncounterManager.instance.StartFight(Go.GetComponent<EnemyCardObject>());
    }
}
