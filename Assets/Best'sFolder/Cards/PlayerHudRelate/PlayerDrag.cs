using UnityEngine;
using DG.Tweening;

public class PlayerDrag : MonoBehaviour
{
    // --- SINGLETON ---
    public static PlayerDrag Instance { get; private set; }

    [Header("Move Settings")]
    public float smoothMoveTime = 0.15f;
    public Ease moveEase = Ease.OutQuad;

    [Header("Rotate Settings")]
    public float smoothRotateTime = 0.25f;

    private Camera mainCamera;

    // Drag state
    private Transform currentTarget;
    private bool isDragging = false;
    private float lockedY;

    // Tweens
    private Tweener moveTween;
    private Tweener rotateTween;

    private void Awake()
    {
        // Setup Singleton Instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    /// <summary>
    /// Call this from any IMouseInteractable object to start dragging it.
    /// </summary>
    public void StartDrag(Transform targetTransform)
    {
        // If we are already dragging an object, ignore new requests
        if (isDragging || targetTransform == null) return;

        currentTarget = targetTransform;
        lockedY = currentTarget.position.y; // Lock height to original Y level
        isDragging = true;
    }

    /// <summary>
    /// Call this when the mouse button is released to stop dragging.
    /// </summary>
    public void StopDrag()
    {
        if (!isDragging) return;

        isDragging = false;
        currentTarget = null;
    }

    private void Update()
    {
        if (isDragging && currentTarget != null)
        {
            MoveAndRotateSmoothly();
            if(Input.GetMouseButtonUp(0))
            {
                if(currentTarget.gameObject.TryGetComponent<CardObj>(out CardObj Co))
                {
                    Co.ResetPos();
                }
                StopDrag();
            }
        }
    }

    private void MoveAndRotateSmoothly()
    {
        // 1. Raycast onto an invisible plane locked at lockedY
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane xzPlane = new Plane(Vector3.up, new Vector3(0, lockedY, 0));

        if (xzPlane.Raycast(ray, out float distanceToPlane))
        {
            Vector3 targetPosition = ray.GetPoint(distanceToPlane);

            // 2. Smooth Move via DOTween
            if (moveTween == null || !moveTween.IsActive())
            {
                moveTween = currentTarget.DOMove(targetPosition, smoothMoveTime)
                    .SetEase(moveEase)
                    .SetAutoKill(false);
            }
            else
            {
                // Update target position smoothly
                moveTween.ChangeEndValue(targetPosition, true).Restart();
            }

            // 3. Smooth Y-axis Rotation towards direction of motion
            Vector3 direction = targetPosition - currentTarget.position;
            direction.y = 0; // Lock rotation to Y axis only

            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(-direction);
                Vector3 targetEuler = targetRotation.eulerAngles;

                if (rotateTween == null || !rotateTween.IsActive())
                {
                    rotateTween = currentTarget.DORotate(targetEuler, smoothRotateTime, RotateMode.Fast)
                        .SetEase(Ease.OutQuad)
                        .SetAutoKill(false);
                }
                else
                {
                    rotateTween.ChangeEndValue(targetEuler, true).Restart();
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (moveTween != null) moveTween.Kill();
        if (rotateTween != null) rotateTween.Kill();
    }
}