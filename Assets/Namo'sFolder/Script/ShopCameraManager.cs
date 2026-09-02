using UnityEngine;

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
        targetPos = shopCameraPos;
        targetVec = shopCameraPos.position;
        targetRot = shopCameraPos.rotation;
        isInShop = true;
        isPausedForFocus = false;

        // Trigger shop generation
        if (ShopManager.Instance != null)
            ShopManager.Instance.GenerateShopItems();
        else
            Debug.LogWarning("[ShopCameraManager] ShopManager.Instance null on GoToShop", this);

        // BGM transition
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayShopBGM();
    }

    public void GoToGame()
    {
        if (gameCameraPos == null)
        {
            Debug.LogWarning("[ShopCameraManager] gameCameraPos not assigned", this);
            return;
        }
        // If focused, exit focus first
        if (ShopFocusManager.Instance != null && ShopFocusManager.Instance.IsFocused)
            ShopFocusManager.Instance.ExitFocus();

        targetPos = gameCameraPos;
        targetVec = gameCameraPos.position;
        targetRot = gameCameraPos.rotation;
        isInShop = false;
        isPausedForFocus = false;

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayGameBGM();
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
