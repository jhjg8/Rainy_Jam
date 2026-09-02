#if false // Disabled duplicate - Namo'sFolder/ShopManager is canonical (fixes CS0101 duplicate)
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }


    private const int ShopSize = 8;
    private const int BaseRankMin = 1;
    private const int BaseRankMax = 10;
    private const int BaseEnchantPower = 6;
    private const int MaxSuitsPerCard = 6;
    private const int PricePerSuit = 3;

    [Header("Shop Positions (Transforms for 8 card slots)")]
    public Transform[] cardSpawnPositions = new Transform[ShopSize];

    [Header("Shop Data (Runtime)")]
    public List<GameObject> shopCards = new List<GameObject>();
    public List<int> cardPrices = new List<int>();

    [Header("Economy")]
    [Tooltip("Player currency for shop purchases")]
    public int playerGold = 100;
    public int startingGold = 100;

    [Header("Entrance Animation")]
    [Range(0.02f, 0.2f)] public float staggerDelay = 0.075f;
    public float entranceDuration = 0.38f;
    public float entranceYOffset = -0.7f;
    public float entranceScaleFrom = 0.6f;
    [Tooltip("Easing for entrance pop")]
    public AnimationCurve entranceCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    public System.Action<int> OnCardPurchased;
    public System.Action OnShopGenerated;

    private Coroutine entranceRoutine;
    private bool isEntranceAnimating = false;
    public bool IsEntranceAnimating => isEntranceAnimating;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
#endif