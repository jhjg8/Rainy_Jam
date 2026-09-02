using UnityEngine;
using System.Collections;

public class ObjectShaker : MonoBehaviour
{
    [Header("Target")]
    public Transform targetTransform;

    [Header("Shake Settings (X, Z & Y Rotation)")]
    public float defaultPosMagnitude = 0.15f;
    public float defaultRotMagnitude = 2.0f;
    public float defaultDuration = 0.2f;

    private Coroutine shakeRoutine;

    private void Awake()
    {
        if (targetTransform == null) targetTransform = transform;
    }

    private void OnValidate()
    {
        if (targetTransform == null) targetTransform = transform;
    }

    public void ShakeDefault()
    {
        Shake(defaultPosMagnitude, defaultRotMagnitude, defaultDuration);
    }

    public void Shake(float posMag, float rotMag, float duration = 0.2f)
    {
        if (targetTransform == null) return;
        if (shakeRoutine != null) StopCoroutine(shakeRoutine);
        shakeRoutine = StartCoroutine(ShakeRoutine(duration, posMag, rotMag));
    }

    private IEnumerator ShakeRoutine(float duration, float posMag, float rotMag)
    {
        Vector3 originalPos = targetTransform.localPosition;
        Quaternion originalRot = targetTransform.localRotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * posMag;
            float z = Random.Range(-1f, 1f) * posMag;
            float yRot = Random.Range(-1f, 1f) * rotMag;

            targetTransform.localPosition = originalPos + new Vector3(x, 0f, z);
            targetTransform.localRotation = originalRot * Quaternion.Euler(0f, yRot, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        targetTransform.localPosition = originalPos;
        targetTransform.localRotation = originalRot;
        shakeRoutine = null;
    }
}
