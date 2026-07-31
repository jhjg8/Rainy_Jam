using UnityEngine;
using DG.Tweening; // Make sure DOTween is installed and set up

public class WorldToCasvas : MonoBehaviour
{
    [Header("Target & Offset")]
    [Tooltip("The 3D GameObject in the world space you want to follow")]
    public Transform worldTarget;
    
    [Tooltip("Offset in World Space (e.g., Y = 2 to float above a character's head)")]
    public Vector3 worldOffset = new Vector3(0, 2f, 0);

    [Header("UI References")]
    [Tooltip("The UI element to move (defaults to the object this script is on)")]
    public RectTransform uiElement;
    
    [Tooltip("Optional: CanvasGroup to fade out the UI when the target is behind the camera")]
    public CanvasGroup canvasGroup;

    [Header("DOTween Settings")]
    [Tooltip("How fast the UI catches up to the target (lower is faster)")]
    public float smoothTime = 0.15f;
    public Ease easeType = Ease.OutQuad;

    private Camera mainCam;
    private Tweener moveTween;

    private void Start()
    {
        // Cache the main camera
        mainCam = Camera.main;

        // Auto-assign the RectTransform if left empty
        if (uiElement == null)
        {
            uiElement = GetComponent<RectTransform>();
        }

        // Auto-assign CanvasGroup if one exists on this object
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
    }

    private void LateUpdate()
    {
        if (worldTarget == null || mainCam == null) return;

        // 1. Calculate the exact 3D position + your desired offset
        Vector3 targetWorldPos = worldTarget.position + worldOffset;

        // 2. Convert World Space to Screen Space
        Vector3 screenPos = mainCam.WorldToScreenPoint(targetWorldPos);

        // 3. Check if the object is behind the camera
        if (screenPos.z < 0)
        {
            // Object is behind us, hide the UI
            if (canvasGroup != null) canvasGroup.alpha = 0f;
            return; 
        }
        else
        {
            // Object is in front of us, show the UI
            if (canvasGroup != null) canvasGroup.alpha = 1f;
        }

        // 4. Smooth Movement using DOTween
        // We use ChangeEndValue so we aren't creating a brand new Tween every single frame (which causes lag)
        if (moveTween == null || !moveTween.IsActive())
        {
            // Create the tween the first time
            moveTween = uiElement.DOMove(screenPos, smoothTime)
                .SetEase(easeType)
                .SetAutoKill(false); // Keep it alive so we can update it
        }
        else
        {
            // Update the destination of the existing tween
            moveTween.ChangeEndValue(screenPos, true).Restart();
        }
    }

    private void OnDestroy()
    {
        // Clean up the tween when this object is destroyed to prevent memory leaks
        if (moveTween != null)
        {
            moveTween.Kill();
        }
    }
}