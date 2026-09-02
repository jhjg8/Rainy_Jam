using UnityEngine;
using TMPro;
using System.Linq;

public class ShopFocusManager : MonoBehaviour
{
    public static ShopFocusManager Instance { get; private set; }

    [Header("Camera Control")]
    [Range(1f, 20f)] public float moveSpeed = 5f;
    public float focusOffsetZ = -2f;
    public float focusOffsetY = 0.35f;
    [Tooltip("Lateral offset to frame card to LEFT of screen (negative = left, leaves right half for UI)")]
    public float focusLateralOffset = -0.95f;
    [Tooltip("Faster lerp when cycling via scroll wheel")]
    public float scrollFocusSpeed = 9f;

    [Header("UI - Right-Side Info Panel")]
    public GameObject focusUIPanel;
    [Tooltip("Legacy combined info (kept for backward compat)")]
    public TextMeshProUGUI cardInfoText;
    public TextMeshProUGUI cardPriceText;

    [Header("Detailed TMP Fields (optional - if assigned, cardInfoText is split)")]
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI cardRankText;
    public TextMeshProUGUI cardPowerText;
    public TextMeshProUGUI cardSuitsText;
    public TextMeshProUGUI cardEnchantedText;
    public TextMeshProUGUI cardGoldText;

    [Header("Buttons (bottom-right)")]
    public UnityEngine.UI.Button purchaseButton;
    public UnityEngine.UI.Button backButton;

    [Header("References")]
    public Transform cameraTransform;

    [Header("Scroll Navigation")]
    public float scrollCooldown = 0.22f;
    public bool wrapAround = true;

    private int focusedIndex = -1;
    private bool isFocused = false;
    private Vector3 preFocusCamPos;
    private Quaternion preFocusCamRot;
    private Vector3 targetCamPos;
    private Quaternion targetCamRot;
    private bool hasPreFocus = false;
    private float lastScrollTime = -999f;
    private float defaultMoveSpeed;

    public int FocusedIndex => focusedIndex;
    public bool IsFocused => isFocused;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (cameraTransform == null)
        {
            if (ShopCameraManager.Instance != null) cameraTransform = ShopCameraManager.Instance.transform;
            else if (Camera.main != null) cameraTransform = Camera.main.transform;
            else cameraTransform = transform;
        }
        if (cameraTransform != null)
        {
            targetCamPos = cameraTransform.position;
            targetCamRot = cameraTransform.rotation;
        }
        defaultMoveSpeed = moveSpeed;

        if (focusUIPanel != null) focusUIPanel.SetActive(false);
        if (purchaseButton != null) purchaseButton.onClick.AddListener(PurchaseFocusedCard);
        if (backButton != null) backButton.onClick.AddListener(ExitFocus);
    }

    private void OnDestroy()
    {
        if (purchaseButton != null) purchaseButton.onClick.RemoveListener(PurchaseFocusedCard);
        if (backButton != null) backButton.onClick.RemoveListener(ExitFocus);
        targetCamPos = transform.position;

        if (focusUIPanel != null)
            focusUIPanel.SetActive(false);
    }

    private void Update()
    {

        if (isFocused)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) { ExitFocus(); return; }
            HandleOutsideClick();
            HandleScrollWheel();
        }

        if (isFocused && ShopCameraManager.Instance != null) return; // delegated
        if (cameraTransform == null) return;
        if (isFocused || hasPreFocus)
        {
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetCamPos, Time.deltaTime * moveSpeed);
            cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, targetCamRot, Time.deltaTime * moveSpeed);
        }
    }

    private void HandleOutsideClick()
    {
        
    
        transform.position = Vector3.Lerp(transform.position, targetCamPos, Time.deltaTime * moveSpeed);

        // Display player coins continuously
        if (playerCoinsText != null && CycleManager.Instance != null)
        {
            playerCoinsText.text = $"Coins: {CycleManager.Instance.Coins}";
        }
    }

    public void FocusOnCard(int index)
    {
        if (!Input.GetMouseButtonDown(0)) return;
        // Ignore click over UI
        if (UnityEngine.EventSystems.EventSystem.current != null && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;

        // Raycast from camera: if we hit the focused card, ignore (second click = purchase handled by ShopCardInput)
        var cam = cameraTransform != null ? cameraTransform.GetComponent<Camera>() : null;
        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 50f))
        focusedIndex = index;

        // Hide other cards
        for (int i = 0; i < ShopManager.Instance.shopCards.Count; i++)

        {
            if (ShopManager.Instance != null && focusedIndex >= 0 && focusedIndex < ShopManager.Instance.shopCards.Count)
            {
                var focusedGO = ShopManager.Instance.shopCards[focusedIndex];
                if (hit.collider != null && hit.collider.gameObject == focusedGO) return;
                // also check child collider
                if (hit.collider != null && hit.collider.transform.IsChildOf(focusedGO.transform)) return;
            }
        }
        // Clicked outside card → exit
        ExitFocus();
    }

    private void HandleScrollWheel()
    {
        float scroll = Input.mouseScrollDelta.y;
        // Fallback for old input
        if (Mathf.Approximately(scroll, 0f)) scroll = Input.GetAxis("Mouse ScrollWheel") * 10f;
        if (Mathf.Abs(scroll) < 0.01f) return;
        if (Time.time - lastScrollTime < scrollCooldown) return;
        lastScrollTime = Time.time;

        int dir = scroll > 0 ? -1 : 1; // wheel up = previous, down = next (natural)
        int next = GetAdjacentAvailableIndex(focusedIndex, dir);
        if (next == -1 || next == focusedIndex) return;

        // Fast seamless transition
        SwitchFocusInstant(next);
    }

    private int GetAdjacentAvailableIndex(int from, int dir)
    {
        if (ShopManager.Instance == null) return -1;
        int count = ShopManager.Instance.shopCards.Count;
        if (count == 0) return -1;

        // Count available cards
        int available = 0;
        for (int i = 0; i < count; i++) if (ShopManager.Instance.shopCards[i] != null) available++;
        if (available <= 1) return from; // nowhere to cycle

        int cur = from;
        for (int step = 0; step < count; step++)
        {
            cur += dir;
            if (wrapAround)
            {
                if (cur < 0) cur = count - 1;
                if (cur >= count) cur = 0;
            }
            else
            {
                if (cur < 0 || cur >= count) return from;
            }
            if (ShopManager.Instance.shopCards[cur] != null) return cur;
            if (cur == from) break; // looped
        }
        return -1;
    }

    private void SwitchFocusInstant(int newIndex)
    {
        // Keep isFocused true, swap visible card and camera target with faster speed
        int oldIndex = focusedIndex;
        if (oldIndex == newIndex) return;

        // Swap visibility: hide old, show new (others stay hidden)
        if (ShopManager.Instance != null)
        {
            for (int i = 0; i < ShopManager.Instance.shopCards.Count; i++)
            {
                var go = ShopManager.Instance.shopCards[i];
                if (go == null) continue;
                // Only the new target visible
                go.SetActive(i == newIndex);
            }
        }

        focusedIndex = newIndex;
        var targetGO = ShopManager.Instance.shopCards[newIndex];
        if (targetGO == null) return;

        Vector3 cardPos = targetGO.transform.position;
        Quaternion cardRot = targetGO.transform.rotation;
        // Off-center offset: lateral (right vector) + up + forward
        Vector3 offset = cardRot * new Vector3(focusLateralOffset, focusOffsetY, focusOffsetZ);
        targetCamPos = cardPos + offset;
        Vector3 dir = (cardPos - targetCamPos).normalized;
        targetCamRot = dir.sqrMagnitude > 0.001f ? Quaternion.LookRotation(dir, Vector3.up) : cardRot;
        Vector3 cardPos = ShopManager.Instance.shopCards[index].transform.position;
        targetCamPos = new Vector3(cardPos.x, cardPos.y, cardPos.z + focusOffsetZ);

        if (focusUIPanel != null) focusUIPanel.SetActive(true);
        // Temporarily faster lerp for scroll
        moveSpeed = scrollFocusSpeed;
        if (ShopCameraManager.Instance != null)
            ShopCameraManager.Instance.SetFocusTarget(targetCamPos, targetCamRot);
        // Restore speed after short delay
        CancelInvoke(nameof(RestoreMoveSpeed));
        Invoke(nameof(RestoreMoveSpeed), 0.35f);

        UpdateDetailsPanel(newIndex);
        if (SoundManager.Instance != null) SoundManager.Instance.PlayInteractCardSFX();
    }

    private void RestoreMoveSpeed() => moveSpeed = defaultMoveSpeed;

    public void FocusOnCard(int index)
    {
        if (ShopManager.Instance == null) return;
        if (index < 0 || index >= ShopManager.Instance.shopCards.Count) return;
        GameObject targetGO = ShopManager.Instance.shopCards[index];
        if (targetGO == null) { if (SoundManager.Instance != null) SoundManager.Instance.PlayDamageSFX(); return; }

        if (cameraTransform != null)
        {
            preFocusCamPos = cameraTransform.position;
            preFocusCamRot = cameraTransform.rotation;
            hasPreFocus = true;
        }
        else if (ShopCameraManager.Instance != null)
        {
            preFocusCamPos = ShopCameraManager.Instance.transform.position;
            preFocusCamRot = ShopCameraManager.Instance.transform.rotation;
            hasPreFocus = true;
        }

        focusedIndex = index;
        isFocused = true;
        moveSpeed = defaultMoveSpeed;

        // Hide non-inspected cards
        for (int i = 0; i < ShopManager.Instance.shopCards.Count; i++)
        {
            var go = ShopManager.Instance.shopCards[i];
            if (go != null) go.SetActive(i == index);
        }

        Vector3 cardPos = targetGO.transform.position;
        Quaternion cardRot = targetGO.transform.rotation;
        Vector3 offset = cardRot * new Vector3(focusLateralOffset, focusOffsetY, focusOffsetZ);
        targetCamPos = cardPos + offset;
        Vector3 dir = (cardPos - targetCamPos).normalized;
        targetCamRot = dir.sqrMagnitude > 0.001f ? Quaternion.LookRotation(dir, Vector3.up) : cardRot;

        if (ShopCameraManager.Instance != null)
            ShopCameraManager.Instance.SetFocusTarget(targetCamPos, targetCamRot);

        if (focusUIPanel != null) focusUIPanel.SetActive(true);
        UpdateDetailsPanel(index);

        if (SoundManager.Instance != null) SoundManager.Instance.PlayInteractCardSFX();
    }

    private void UpdateDetailsPanel(int index)
    {
        if (ShopManager.Instance == null) return;
        var go = ShopManager.Instance.shopCards[index];
        if (go == null) return;
        CardObj cardObj = go.GetComponent<CardObj>();
        Card card = cardObj != null ? cardObj.card : null;

        int price = ShopManager.Instance.GetPrice(index);
        bool canAfford = ShopManager.Instance.CanAfford(index);

        // Derived fields
        string nameStr = card != null ? card.cardname : $"Card {index}";
        int rank = card != null ? card.Rank : 0;
        int power = rank; // Power == Rank per spec
        string suitsStr = "";
        bool isEnchanted = false;
        int suitCount = 0;
        if (card != null && card.suits != null)
        {
            suitCount = card.suits.Count;
            for (int s = 0; s < card.suits.Count; s++)
            {
                var suit = card.suits[s];
                suitsStr += suit != null ? suit.Name : "?";
                if (s < card.suits.Count - 1) suitsStr += ", ";
            }
            // Enchanted = more than 1 suit AND at least one special (beyond first 4)
            if (suitCount > 1)
            {
                // Heuristic: if count>1 consider enchanted; more precise if specials exist
                isEnchanted = true;
                // Try to detect special by checking allsuits reference if available
                if (CardManager.instance != null && CardManager.instance.allsuits != null && CardManager.instance.allsuits.Count > 4)
                {
                    isEnchanted = false;
                    for (int i = 0; i < card.suits.Count; i++)
                    {
                        if (CardManager.instance.allsuits.IndexOf(card.suits[i]) >= 4) { isEnchanted = true; break; }
                    }
                }
            }
        }
        string enchantedStr = isEnchanted ? "Yes" : "No";
        string priceStr = $"{price} Coins";

        // Detailed TMP fields (if assigned)
        if (cardNameText != null) cardNameText.text = nameStr;
        if (cardRankText != null) cardRankText.text = $"Rank: {rank}";
        if (cardPowerText != null) cardPowerText.text = $"Power: {power}";
        if (cardSuitsText != null) cardSuitsText.text = $"Suits: {suitsStr} ({suitCount})";
        if (cardEnchantedText != null)
        {
            cardEnchantedText.text = $"Enchanted: {enchantedStr}";
            cardEnchantedText.color = isEnchanted ? new Color(0.6f, 0.85f, 1f) : Color.white;
        }
        if (cardGoldText != null)
        {
            cardGoldText.text = priceStr;
            cardGoldText.color = canAfford ? Color.white : Color.red;
        }

        // Fallback combined
        if (cardInfoText != null)
        {
            cardInfoText.text = $"{nameStr}\nRank: {rank}  Power: {power}\nSuits: {suitsStr}\nEnchanted: {enchantedStr}";
        }
        if (cardPriceText != null)
        {
            cardPriceText.text = priceStr;
            cardPriceText.color = canAfford ? Color.white : Color.red;
        }
        if (cardGoldText == null && cardPriceText != null)
        {
            // use cardPriceText as gold display
        }

        if (purchaseButton != null) purchaseButton.interactable = canAfford;
    }

    public void ExitFocus()
    {
        if (!isFocused && focusedIndex == -1 && focusUIPanel != null && !focusUIPanel.activeSelf) return;
        isFocused = false;
        moveSpeed = defaultMoveSpeed;

        if (ShopManager.Instance != null)
            for (int i = 0; i < ShopManager.Instance.shopCards.Count; i++)
            {
                var go = ShopManager.Instance.shopCards[i];
                if (go != null && !go.activeSelf) go.SetActive(true);
            }

        if (ShopCameraManager.Instance != null && ShopCameraManager.Instance.shopCameraPos != null)
        {
            ShopCameraManager.Instance.ReturnToShopPosition();
            targetCamPos = ShopCameraManager.Instance.shopCameraPos.position;
            targetCamRot = ShopCameraManager.Instance.shopCameraPos.rotation;
        }
        else if (hasPreFocus && cameraTransform != null)
        {
            targetCamPos = preFocusCamPos;
            targetCamRot = preFocusCamRot;
        }

        if (focusUIPanel != null) focusUIPanel.SetActive(false);
        focusedIndex = -1;
        CancelInvoke(nameof(RestoreMoveSpeed));
        if (SoundManager.Instance != null) SoundManager.Instance.PlayInteractCardSFX();
    }

    public void PurchaseFocusedCard()
    {
        if (!isFocused || focusedIndex < 0) return;
        if (ShopManager.Instance == null) return;
        bool success = ShopManager.Instance.TryBuyCard(focusedIndex);
        if (success) { if (isFocused) ExitFocus(); }
        else
        {
            var target = focusUIPanel != null ? focusUIPanel.GetComponent<ObjectShaker>() : null;
            if (target == null && focusUIPanel != null) target = focusUIPanel.AddComponent<ObjectShaker>();
            if (target != null) target.Shake(0.05f, 2f, 0.2f);
            var camShake = FindAnyObjectByType<CameraShake>();
            if (camShake != null) camShake.Shake(0.08f, 1.5f, 0.15f);
            // Refresh panel to show red price
            UpdateDetailsPanel(focusedIndex);
            int price = ShopManager.Instance.cardPrices[index];
            cardPriceText.text = $"Price: {price} Coins";
        }
    }

    public void BuyFocusedCard()
    {
        if (focusedIndex < 0 || focusedIndex >= ShopManager.Instance.shopCards.Count) return;

        int price = ShopManager.Instance.cardPrices[focusedIndex];
        if (CycleManager.Instance != null && CycleManager.Instance.Coins >= price)
        {
            CycleManager.Instance.Coins -= price;
            ShopManager.Instance.BuyCard(focusedIndex);
            Unfocus();
        }
    }

    public void Unfocus()
    {
        if (focusUIPanel != null) focusUIPanel.SetActive(false);
        focusedIndex = -1;

        if (ShopManager.Instance != null)
        {
            for (int i = 0; i < ShopManager.Instance.shopCards.Count; i++)
            {
                if (ShopManager.Instance.shopCards[i] != null)
                {
                    ShopManager.Instance.shopCards[i].SetActive(true);
                }
            }
        }

        if (ShopCameraManager.Instance != null && ShopCameraManager.Instance.shopCameraPos != null)
        {
            targetCamPos = ShopCameraManager.Instance.shopCameraPos.position;
        }
    }

    public void OnBackButton() => ExitFocus();
    public bool IsCardFocused(int index) => isFocused && focusedIndex == index;
}