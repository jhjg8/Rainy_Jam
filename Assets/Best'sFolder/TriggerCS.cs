using UnityEngine;

public class TriggerCS : MonoBehaviour
{
    [Header("Shake Target (Main Camera)")]
    public CameraShake cameraShake;
    public ObjectShaker objectShaker;

    [Header("Shake Presets")]
    public float posMagnitude = 0.3f;
    public float rotMagnitude = 5f;
    public float duration = 0.25f;

    private void Reset()
    {
        if (cameraShake == null) cameraShake = FindAnyObjectByType<CameraShake>();
    }

    // Called by external events (e.g., Player hit)
    public void OnPlayerHit()
    {
        TriggerShake();
    }

    public void TriggerShake()
    {
        if (cameraShake != null)
            cameraShake.Shake(posMagnitude, rotMagnitude, duration);
        else
        {
            var cam = Camera.main != null ? Camera.main.GetComponent<CameraShake>() : null;
            if (cam != null) cam.Shake(posMagnitude, rotMagnitude, duration);
        }

        if (objectShaker != null)
            objectShaker.Shake(posMagnitude, rotMagnitude, duration);
    }

    // Generic hook for shop purchase success
    public void OnPurchaseSuccess()
    {
        if (cameraShake != null) cameraShake.Shake(0.12f, 2f, 0.2f);
    }
}
