using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
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
    public List<Texture> DiamondCard;
    public List<Texture> SpadesCard;
    public List<Texture> HeartCard;
    public List<Texture> ClubCard;

    public int DificaultyLevel = 1;
    public static EncounterManager instance;

    public Func<int, int> CalculateEnemyDamage;
    public Func<int, int> CalculatePlayerDamage;

    public int Defendpoint;
    public TextMeshProUGUI text;

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
            if (i == 0)
                c.card.suits.Add(CardManager.instance.allsuits[UnityEngine.Random.Range(0, 4)]);

            else
            {
                while (true) {
                    Suit s = CardManager.instance.allsuits[UnityEngine.Random.Range(4, CardManager.instance.allsuits.Count)];
                    if (s == null) break;
                    if (!c.card.suits.Contains(s)) break;
                }
                c.card.suits.Add(CardManager.instance.allsuits[UnityEngine.Random.Range(4, CardManager.instance.allsuits.Count)]);
            }
        }
        Suit firstSuit = c.card.suits[0];

        if (firstSuit == CardManager.instance.allsuits[0])
        {
            card.GetComponent<MeshRenderer>().materials[0].SetTexture("_mt", ClubCard[UnityEngine.Random.Range(0, ClubCard.Count)]);
        }
        else if (firstSuit == CardManager.instance.allsuits[1])
        {
            card.GetComponent<MeshRenderer>().materials[0].SetTexture("_mt", DiamondCard[UnityEngine.Random.Range(0, DiamondCard.Count)]);
        }
        else if (firstSuit == CardManager.instance.allsuits[2])
        {
            card.GetComponent<MeshRenderer>().materials[0].SetTexture("_mt", HeartCard[UnityEngine.Random.Range(0, HeartCard.Count)]);
        }
        else if (firstSuit == CardManager.instance.allsuits[3])
        {
            card.GetComponent<MeshRenderer>().materials[0].SetTexture("_mt", SpadesCard[UnityEngine.Random.Range(0, SpadesCard.Count)]);
        }
        int points = UnityEngine.Random.Range(DificaultyLevel - 5, DificaultyLevel + 5) - suitsCount + 5;
        int split = UnityEngine.Random.Range(DificaultyLevel / 2, points);
        c.card.Health = (split * 10);
        c.card.Power = ((points - split) * 4);

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
