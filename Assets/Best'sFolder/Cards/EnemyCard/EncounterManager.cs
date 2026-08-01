using System;
using UnityEngine;
public struct CardAttackerEvent
{
    public Card Card;
    public EnemyCardObject Enemy;
}

public struct CardDefenderEvent
{
    public Card Card;
    public EnemyCardObject Enemy;
}
public class EncounterManager : MonoBehaviour
{
    public bool IsFighting;
    public EnemyCardObject EnemyCard;

    public HandOrganizer Ho;
    [SerializeField] private Transform enemypos;

    public EnemyCardObject CurrentEnemy;
    

    public int DificaultyLevel = 1;
    public static EncounterManager instance;

    public Func<int, int> CalculateEnemyDamage;
    public Func<int, int> CalculatePlayerDamage;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        Ho = HandOrganizer.Instance;
    }
    public void StartFight()
    {
        CurrentEnemy = CreateEnemyCard(enemypos.position).GetComponent<EnemyCardObject>();
        EnemyCard enemy = CurrentEnemy.GetComponent<EnemyCard>();
        foreach (var suit in enemy.suits)
        {
            suit.EnableForEnemy();
        }
    }
    public GameObject CreateEnemyCard(Vector3 pos)
    {
        var card = Instantiate(EnemyCard.gameObject,pos,Quaternion.identity);
        EnemyCardObject c = card.GetComponent<EnemyCardObject>();
        c.Belong = pos;
        bool isbreake = false;
        int suitsCount = 1;
        while (!isbreake)
        {
            int chance = UnityEngine.Random.Range(0, 100);
            if (chance < Mathf.Clamp((25 + DificaultyLevel), 25, 50))
            {
                suitsCount++;
            }
            else if (suitsCount >= 6)
            {
                isbreake = true;
            }
            else
            {
                isbreake = true;
            }
        }
        for (int i = 0;i < suitsCount; i++)
        {
            c.card.suits.Add(CardManager.instance.allsuits[UnityEngine.Random.Range(0, CardManager.instance.allsuits.Count)]);
        }
        return card;
    }
    public void Endfight()
    {
        EnemyCard enemy = CurrentEnemy.GetComponent<EnemyCard>();
        foreach (var suit in enemy.suits)
        {
            suit.DisableForEnemy();
        }
        DificaultyLevel++;
    }
    public int CalculateEnemyDmg(int dmg)
    {
        if (CalculateEnemyDamage == null) return dmg;
        int newvalue = dmg;
        foreach (Func<int, int> handler in CalculateEnemyDamage.GetInvocationList())
        {
            newvalue = handler.Invoke(newvalue);
        }
        return newvalue;
    }
    public int CalculatePlayerdmg(int dmg)
    {
        if (CalculatePlayerDamage == null) return dmg;
        int newvalue = dmg;
        foreach (Func<int, int> handler in CalculatePlayerDamage.GetInvocationList())
        {
            newvalue = handler.Invoke(newvalue);
        }
        return newvalue;
    }
}
