using UnityEngine;
using System.Collections;

/// <summary>
/// Shop card interaction: click → Focus, hover → lift/scale/glow, entrance gating.
/// No friend-folder interface dependency (pure OnMouse* + collider).
/// Attached automatically by ShopManager.
/// </summary>
public class ShopCardInput : MonoBehaviour
{
    [Tooltip("Index in ShopManager.shopCards")]
    public int shopIndex = -1;

    [Header("Hover Feedback")]
    public float hoverLift = 0.18f;
    public float hoverScaleFactor = 1.08f;
    public float hoverDuration = 0.13f;
    public bool enableGlow = true;
    public Color glowColor = new Color(1f, 0.92f, 0.5f, 1f);

    private CardObj cardObj;
    private Collider col;
    private Renderer rend;
    private Vector3 basePos;
    private Vector3 baseScale = Vector3.one;
    private Quaternion baseRot;
    private bool hasBase = false;
    private bool isHovering = false;
    private bool entranceDone = false;
    private Coroutine hoverRoutine;
    private Material originalMat;
    private MaterialPropertyBlock glowBlock;

    private void Awake()
    {
        cardObj = GetComponent<CardObj>();
        col = GetComponent<Collider>();
        if (col == null) col = GetComponentInChildren<Collider>();
        rend = GetComponent<Renderer>();
        if (rend == null) rend = GetComponentInChildren<Renderer>();
        if (rend != null) originalMat = rend.material;
        glowBlock = new MaterialPropertyBlock();
    }

    private void Start()
    {
        if (col == null)
        {
            var bc = gameObject.AddComponent<BoxCollider>();
            bc.size = new Vector3(1f, 0.12f, 1.5f);
            bc.isTrigger = false;
            bc.center = Vector3.zero;
            col = bc;
        }
        // Capture base after entrance start pos will be overwritten later - will update on SetEntranceState(true)
        if (!hasBase)
        {
            basePos = transform.position;
            baseRot = transform.rotation;
            baseScale = transform.localScale;
            hasBase = true;
        }
    }

    public void SetEntranceState(bool done)
    {
        entranceDone = done;
        if (done)
        {
            basePos = transform.position;
            baseRot = transform.rotation;
            baseScale = Vector3.one;
            hasBase = true;
            if (col != null) col.enabled = true;
        }
        else
        {
            if (col != null) col.enabled = false;
            isHovering = false;
        }
    }

    private bool CanInteract()
    {
        if (!entranceDone) return false;
        if (ShopManager.Instance != null && ShopManager.Instance.IsEntranceAnimating) return false;
        if (ShopFocusManager.Instance != null && ShopFocusManager.Instance.IsFocused) return false;
        if (ShopCameraManager.Instance != null && !ShopCameraManager.Instance.IsInShop) return false;
        return true;
    }

    // Unity hover callbacks (require Collider + Graphics Raycaster for 3D)
    private void OnMouseEnter()
    {
        if (!CanInteract()) return;
        if (isHovering) return;
        isHovering = true;
        if (hoverRoutine != null) StopCoroutine(hoverRoutine);
        hoverRoutine = StartCoroutine(HoverTransition(true));
        if (SoundManager.Instance != null) SoundManager.Instance.PlayInteractCardSFX();
    }

    private void OnMouseExit()
    {
        if (!isHovering) return;
        isHovering = false;
        if (hoverRoutine != null) StopCoroutine(hoverRoutine);
        hoverRoutine = StartCoroutine(HoverTransition(false));
    }

    private IEnumerator HoverTransition(bool entering)
    {
        Vector3 startPos = transform.position;
        Vector3 startScale = transform.localScale;
        Quaternion startRot = transform.rotation;

        Vector3 targetPos = entering ? basePos + Vector3.up * hoverLift : basePos;
        Vector3 targetScale = entering ? baseScale * hoverScaleFactor : baseScale;
        Quaternion targetRot = baseRot;

        float elapsed = 0f;
        float dur = hoverDuration;
        while (elapsed < dur)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / dur);
            float smooth = Mathf.SmoothStep(0f, 1f, t);
            transform.position = Vector3.Lerp(startPos, targetPos, smooth);
            transform.localScale = Vector3.Lerp(startScale, targetScale, smooth);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, smooth);
            // Glow lerp
            if (enableGlow && rend != null)
            {
                float glowT = entering ? smooth : 1f - smooth;
                ApplyGlow(glowT * 0.35f);
            }
            yield return null;
        }
        transform.position = targetPos;
        transform.localScale = targetScale;
        transform.rotation = targetRot;
        if (enableGlow && rend != null) ApplyGlow(entering ? 0.35f : 0f);
        hoverRoutine = null;
    }

    private void ApplyGlow(float intensity)
    {
        if (rend == null) return;
        // Try emission glow via property block (non-destructive)
        rend.GetPropertyBlock(glowBlock);
        // _EmissionColor is standard; fallback to _BaseColor tint
        if (rend.sharedMaterial != null && rend.sharedMaterial.HasProperty("_EmissionColor"))
        {
            Color col = Color.Lerp(Color.black, glowColor, intensity);
            glowBlock.SetColor("_EmissionColor", col);
            rend.SetPropertyBlock(glowBlock);
        }
    }

    private void OnDisable()
    {
        // Reset hover state when hidden (focus hide)
        isHovering = false;
        if (hoverRoutine != null) { StopCoroutine(hoverRoutine); hoverRoutine = null; }
        if (hasBase)
        {
            transform.position = basePos;
            transform.localScale = baseScale;
            transform.rotation = baseRot;
        }
        if (rend != null) ApplyGlow(0f);
    }

    // Click (OnMouseDown is more reliable than OnMouseUp for shop)
    private void OnMouseDown()
    {
        if (!entranceDone) return;
        if (ShopManager.Instance != null && ShopManager.Instance.IsEntranceAnimating) return;
        if (ShopCameraManager.Instance != null && !ShopCameraManager.Instance.IsInShop) return;
        if (ShopFocusManager.Instance != null && ShopFocusManager.Instance.IsFocused)
        {
            // Click outside focused card will be handled by ShopFocusManager, ignore here
            return;
        }
        HandleClick();
    }

    private void HandleClick()
    {
        if (ShopManager.Instance == null || ShopFocusManager.Instance == null) return;
        if (shopIndex < 0 || shopIndex >= ShopManager.Instance.shopCards.Count)
        {
            int idx = ShopManager.Instance.GetCardIndex(gameObject);
            if (idx >= 0) shopIndex = idx; else return;
        }
        var go = ShopManager.Instance.shopCards[shopIndex];
        if (go == null) return;
        if (go != gameObject)
        {
            int idx = ShopManager.Instance.GetCardIndex(gameObject);
            if (idx >= 0) shopIndex = idx; else return;
        }
        if (ShopFocusManager.Instance.IsFocused && ShopFocusManager.Instance.FocusedIndex == shopIndex)
        {
            ShopFocusManager.Instance.PurchaseFocusedCard();
            return;
        }
        if (ShopFocusManager.Instance.IsFocused)
            ShopFocusManager.Instance.ExitFocus();

        // Reset hover before focus
        isHovering = false;
        if (rend != null) ApplyGlow(0f);

        ShopFocusManager.Instance.FocusOnCard(shopIndex);
        if (cardObj != null) cardObj.canmove = false;
    }

    public void SetIndex(int idx) => shopIndex = idx;

    // Sync base after external move (e.g., after focus return)
    public void RefreshBase()
    {
        basePos = transform.position;
        baseRot = transform.rotation;
        baseScale = transform.localScale;
        hasBase = true;
    }
}
