using System;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
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

public struct EnemyDiedEvent
{
    public Card Card;
    public EnemyCardObject Enemy;
}
public class EncounterManager : MonoBehaviour
{
    public bool IsFighting;
    public EnemyCardObject EnemyCard;

    public HandOrganizer Ho;
    [SerializeField] public Transform enemypos;

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
    public int DamagePoint;
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
    float timer = 0;
    public void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                text.alpha = timer / 1.5f;
            }
        }
        else
        {
            text.gameObject.SetActive(false);
        }
    }
    public void UpdateUI(bool isblue)
    {
        timer = 1.5f;
        text.gameObject.SetActive(true);
        if (isblue)
        {
            text.DOColor( Color.blue, 0.25f).SetEase(Ease.OutQuad);
            text.text = $"DefendPoint: {Defendpoint}";
        }
        else
        {
            text.DOColor(Color.red, 0.25f).SetEase(Ease.OutQuad);
            text.text = $"EnemyDmg: {DamagePoint}";
        }
    }
    public void StartFight(EnemyCardObject Eo)
    {
        CurrentEnemy = Eo;
        CurrentEnemy.Belong = enemypos.position;
        EnemyCard enemy = CurrentEnemy.GetComponent<EnemyCard>();
        foreach (var suit in enemy.suits)
        {
            suit.EnableForEnemy();
        }
    }
    public GameObject CreateEnemyCard(Vector3 pos)
    {
        var card = Instantiate(EnemyCard.gameObject,pos, Quaternion.Euler(0, 180f, 0));
        EnemyCardObject c = card.GetComponent<EnemyCardObject>();
        c.Belong = pos;
        card.name = "EnemyCard";
        bool isbreake = false;
        int suitsCount = 1;
        while (!isbreake)
        {
            int chance = UnityEngine.Random.Range(0, 100);
            if (chance < Mathf.Clamp((25 + DificaultyLevel), 25, 50))
            {
                suitsCount++;
                if (suitsCount >= 6) isbreake = true;
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
                int maxTries = 50;
                while (maxTries > 0) {
                    maxTries--;
                    Suit s = CardManager.instance.allsuits[UnityEngine.Random.Range(4, CardManager.instance.allsuits.Count)];
                    if (s == null) continue;
                    if (!c.card.suits.Contains(s)) {
                        c.card.suits.Add(s);
                        break;
                    }
                }
            }
        }
        Suit firstSuit = c.card.suits[0];
        c.card.cardname = CardManager.instance.names[UnityEngine.Random.Range(0, CardManager.instance.names.Length)];
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
        int split = UnityEngine.Random.Range((DificaultyLevel / 4), points) + 1;
        c.card.Health = (split * 9);
        c.card.Power = ((int)MathF.Max((float)(points - split),1f) * 4);
        c.M = card.GetComponent<MeshRenderer>().materials[0].GetTexture("_mt");
        c.CoinReward = points * 2;
        card.SetActive(true);
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
        if (CycleManager.Instance != null)
        {
            int bonusPerWin = 20; // ensures you can afford 5-coin cards
            CycleManager.Instance.Coins += CurrentEnemy.CoinReward + bonusPerWin;
            Debug.Log($"[Shop] Boss defeated +{CurrentEnemy.CoinReward}+{bonusPerWin} bonus = {CycleManager.Instance.Coins} Coins");
            CycleManager.Instance.OnBossDefeated();
        }
        if (CurrentEnemy != null) CurrentEnemy.gameObject.SetActive(false);
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
