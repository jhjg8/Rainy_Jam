using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    [Header("Position Shake (X, Z)")]
    public float defaultPosMagnitude = 0.15f;
    [Header("Rotation Shake (Y Angle)")]
    public float defaultRotMagnitude = 2.0f;
    [Header("Duration")]
    public float defaultDuration = 0.2f;

    private Coroutine shakeRoutine;
    private Vector3 initialLocalPos;
    private Quaternion initialLocalRot;
    private bool hasInitial = false;

    private void Awake()
    {
        initialLocalPos = transform.localPosition;
        initialLocalRot = transform.localRotation;
        hasInitial = true;
    }

    public void ShakeDefault()
    {
        Shake(defaultPosMagnitude, defaultRotMagnitude, defaultDuration);
    }

    public void Shake(float posMagnitude, float rotMagnitude, float duration = 0.2f)
    {
        if (shakeRoutine != null) StopCoroutine(shakeRoutine);
        shakeRoutine = StartCoroutine(ShakeRoutine(duration, posMagnitude, rotMagnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float posMag, float rotMag)
    {
        Vector3 originalPos = hasInitial ? initialLocalPos : transform.localPosition;
        Quaternion originalRot = hasInitial ? initialLocalRot : transform.localRotation;
        // Capture current as baseline (supports mid-shake restart)
        originalPos = transform.localPosition;
        originalRot = transform.localRotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * posMag;
            float z = Random.Range(-1f, 1f) * posMag;
            transform.localPosition = originalPos + new Vector3(x, 0f, z);

            float yRot = Random.Range(-1f, 1f) * rotMag;
            transform.localRotation = originalRot * Quaternion.Euler(0f, yRot, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
        transform.localRotation = originalRot;
        shakeRoutine = null;
    }

    // Allow external reset if camera moved externally
    public void ResetBaseline()
    {
        initialLocalPos = transform.localPosition;
        initialLocalRot = transform.localRotation;
    }
}
