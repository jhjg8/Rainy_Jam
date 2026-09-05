using UnityEngine;
using EasyTransition;
using DG.Tweening;

public class ShopCameraManager : MonoBehaviour
{
    public static ShopCameraManager Instance { get; private set; }

    [Header("Camera Positions")]
    public Transform gameCameraPos;
    public Transform shopCameraPos;

    [Header("Movement Settings")]
    [Range(1f, 20f)] public float moveSpeed = 5f;
    [Tooltip("Snap threshold to consider arrival")]
    public float snapThreshold = 0.01f;

    [Header("UI Panels (auto-hide hand/fight in shop)")]
    public GameObject shopUIPanel;
    public GameObject fightUIPanel;
    [Tooltip("Optional: hide Hand/Field root objects in shop to fix 'hand still showing'")]
    public GameObject handRoot;
    public GameObject fieldRoot;

    [Header("Transition (same as Main Menu Play)")]
    [Tooltip("Drag same TransitionSettings used by Main Menu SceneTransitionManager for smooth circle wipe")]
    public TransitionSettings shopTransition;
    public float transitionDelay = 0f;
    public bool useTransition = true;

    private Transform targetPos;
    private bool isInShop = false;
    private bool isPausedForFocus = false;
    private Quaternion targetRot;
    private Vector3 targetVec;

    public bool IsInShop => isInShop;
    public bool IsPausedForFocus => isPausedForFocus;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (gameCameraPos != null)
        {
            targetPos = gameCameraPos;
            targetVec = gameCameraPos.position;
            targetRot = gameCameraPos.rotation;
            transform.SetPositionAndRotation(targetVec, targetRot);
        }
        else
        {
            targetVec = transform.position;
            targetRot = transform.rotation;
        }
    }

    private void Update()
    {
        if (isPausedForFocus) return;
        if (targetPos != null)
        {
            targetVec = targetPos.position;
            targetRot = targetPos.rotation;
        }

        // Smooth lerp with snap
        transform.position = Vector3.Lerp(transform.position, targetVec, Time.deltaTime * moveSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * moveSpeed);

        if (Vector3.Distance(transform.position, targetVec) < snapThreshold)
            transform.position = targetVec;
        if (Quaternion.Angle(transform.rotation, targetRot) < 0.1f)
            transform.rotation = targetRot;
    }

    public void ToggleShop()
    {
        // Route through CycleManager for correct win->shop->next round flow
        if (CycleManager.Instance != null)
        {
            CycleManager.Instance.ChangePhase();
            return;
        }
        if (isInShop) GoToGame();
        else GoToShop();
    }

    public void GoToShop()
    {
        if (shopCameraPos == null)
        {
            Debug.LogWarning("[ShopCameraManager] shopCameraPos not assigned", this);
            return;
        }

        // Show shop UI immediately (behind circle) so it doesn't appear "inactive for few sec"
        if (shopUIPanel != null) shopUIPanel.SetActive(true);
        if (fightUIPanel != null) fightUIPanel.SetActive(false);
        SetHandVisibility(false);

        // Generate shop cards BEFORE transition so they are ready behind circle
        if (ShopManager.Instance != null)
        {
            bool hasPos = ShopManager.Instance.cardSpawnPositions != null && ShopManager.Instance.cardSpawnPositions.Length == 8 && ShopManager.Instance.cardSpawnPositions[0] != null;
            if (!hasPos) Debug.LogWarning("[ShopCameraManager] ShopManager.cardSpawnPositions not assigned! Cards will spawn at 0,0,0 and be invisible from shop cam.", this);
            ShopManager.Instance.GenerateShopItems();
        }

        System.Action doCamera = () =>
        {
            targetPos = shopCameraPos;
            targetVec = shopCameraPos.position;
            targetRot = shopCameraPos.rotation;
            isInShop = true;
            isPausedForFocus = false;

            if (CycleManager.Instance != null) CycleManager.Instance.CurrentPhase = CycleManager.Phase.BuyPhase;

            if (SoundManager.Instance != null)
                SoundManager.Instance.PlayShopBGM();
        };

        // If transition already running (e.g., spam click), do direct move to avoid freeze
        if (useTransition && shopTransition != null && TransitionManager.Instance() != null && !TransitionManager.Instance().IsRunning)
        {
            var mgr = TransitionManager.Instance();
            UnityEngine.Events.UnityAction onCut = null;
            onCut = () => { doCamera(); mgr.onTransitionCutPointReached -= onCut; };
            mgr.onTransitionCutPointReached += onCut;
            mgr.Transition(shopTransition, transitionDelay);
        }
        else
        {
            doCamera();
        }
    }

    public void GoToGame()
    {
        if (gameCameraPos == null)
        {
            Debug.LogWarning("[ShopCameraManager] gameCameraPos not assigned", this);
            return;
        }
        if (ShopFocusManager.Instance != null && ShopFocusManager.Instance.IsFocused)
            ShopFocusManager.Instance.ExitFocus();

        System.Action doGame = () =>
        {
            targetPos = gameCameraPos;
            targetVec = gameCameraPos.position;
            targetRot = gameCameraPos.rotation;
            isInShop = false;
            isPausedForFocus = false;

            if (CycleManager.Instance != null) CycleManager.Instance.CurrentPhase = CycleManager.Phase.FightPhase;

            if (shopUIPanel != null) shopUIPanel.SetActive(false);
            if (fightUIPanel != null) fightUIPanel.SetActive(true);
            SetHandVisibility(true);

            if (ShopManager.Instance != null)
                ShopManager.Instance.ClearShop();

            if (SoundManager.Instance != null)
                SoundManager.Instance.PlayGameBGM();
        };

        if (useTransition && shopTransition != null && TransitionManager.Instance() != null && !TransitionManager.Instance().IsRunning)
        {
            var mgr = TransitionManager.Instance();
            UnityEngine.Events.UnityAction onCut = null;
            onCut = () => { doGame(); mgr.onTransitionCutPointReached -= onCut; };
            mgr.onTransitionCutPointReached += onCut;
            mgr.Transition(shopTransition, transitionDelay);
        }
        else
        {
            doGame();
        }
    }

    private void SetHandVisibility(bool visible)
    {
        // FIX: Only hide fight UI roots, never hide shop roots
        if (handRoot == null && HandOrganizer.Instance != null) handRoot = HandOrganizer.Instance.gameObject;
        if (fieldRoot == null && Field.Instance != null) fieldRoot = Field.Instance.gameObject;
        // Safety: if handRoot mis-assigned to non-HandOrganizer, fix it
        if (handRoot != null && handRoot.GetComponent<HandOrganizer>() == null && HandOrganizer.Instance != null) handRoot = HandOrganizer.Instance.gameObject;
        if (handRoot != null) handRoot.SetActive(visible);
        if (fieldRoot != null) fieldRoot.SetActive(visible);
        if (DisplayField.Instance != null) DisplayField.Instance.gameObject.SetActive(visible);
        // Do NOT per-card SetActive(false) — it made hand "gone for some rounds" because new draws stay hidden
        // Hand cards stay visible via root, just hidden when root hidden
        if (!visible && Indicator.instance != null) Indicator.instance.cancle();
    }

    // Called by ShopFocusManager to take over camera
    public void SetFocusTarget(Vector3 position, Quaternion rotation)
    {
        targetPos = null; // detach from transform
        targetVec = position;
        targetRot = rotation;
        isPausedForFocus = false; // still lerp, but to custom position
    }

    public void PauseForFocus(bool pause) => isPausedForFocus = pause;

    public void ReturnToShopPosition()
    {
        if (shopCameraPos == null) return;
        targetPos = shopCameraPos;
        targetVec = shopCameraPos.position;
        targetRot = shopCameraPos.rotation;
        isPausedForFocus = false;
    }

    // Legacy API
    public bool IsInShopLegacy() => isInShop;
    public bool IsInShopMethod() => isInShop;
}
