using UnityEngine;

public class PlayerSelect : MonoBehaviour
{
    [Header("Raycast Settings")]
    public Camera mainCamera;
    public float rayDistance = 100f;
    public LayerMask interactableLayer = Physics.DefaultRaycastLayers;

    private IMouseInteractAble currentInteractable;

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        PerformMouseRaycast();
    }

    private void PerformMouseRaycast()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, interactableLayer))
        {
            IMouseInteractAble hitInteractable = hit.collider.GetComponent<IMouseInteractAble>();

            if (hitInteractable != null)
            {
                // Check for entering a new object
                if (hitInteractable != currentInteractable)
                {
                    if (currentInteractable != null)
                    {
                        currentInteractable.MouseExit();
                    }

                    currentInteractable = hitInteractable;
                    currentInteractable.MouseEnter();
                }

                if (Input.GetMouseButtonDown(0))
                {
                    currentInteractable.MouseClick();
                }

                if (Input.GetMouseButtonUp(0))
                {
                    currentInteractable.MouseUp();
                }
            }
            else
            {
                ClearCurrentInteractable();
            }
        }
        else
        {
            ClearCurrentInteractable();
        }
    }

    private void ClearCurrentInteractable()
    {
        if (currentInteractable != null)
        {
            currentInteractable.MouseExit();
            currentInteractable = null;
        }
    }
}