using UnityEngine;
using TMPro;
using System.Collections;

public class ShopDemoManager : MonoBehaviour
{
    public static ShopDemoManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI popUpMinusText;

    [Header("Data")]
    public int playerCoins = 100;

    private void Awake() => Instance = this;

    private void Start() => UpdateCoinUI();

    // ปุ่มกดเปิด/ปิดร้านซ้ายบน
    public void ToggleShop()
    {
        bool isActive = !gameObject.activeSelf;
        gameObject.SetActive(isActive);

        if (!isActive && ShopFocusManager.Instance != null)
        {
            ShopFocusManager.Instance.ExitFocus();
        }
    }

    public bool SpendCoinsWithFX(int amount)
    {
        if (playerCoins >= amount)
        {
            playerCoins -= amount;
            StartCoroutine(CoinDeductFXRoutine(amount));
            return true;
        }
        return false;
    }

    private IEnumerator CoinDeductFXRoutine(int amount)
    {
        if (coinText != null)
        {
            coinText.text = $"Coins: {playerCoins}";
            coinText.color = Color.red;
        }
        if (popUpMinusText != null)
        {
            popUpMinusText.text = $"-{amount}";
            popUpMinusText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(1f);

        if (coinText != null) coinText.color = Color.white;
        if (popUpMinusText != null) popUpMinusText.gameObject.SetActive(false);
    }

    private void UpdateCoinUI()
    {
        if (coinText != null) coinText.text = $"Coins: {playerCoins}";
    }
}