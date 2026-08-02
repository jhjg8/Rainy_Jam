using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    [Header("Position Shake (X, Z)")]
    public float defaultPosMagnitude = 0.15f; // ความแรงสั่นตำแหน่งแกน X, Z

    [Header("Rotation Shake (Y Angle)")]
    public float defaultRotMagnitude = 2.0f;  // ความแรงหมุนรอบแกน Y

    [Header("Duration")]
    public float defaultDuration = 0.2f;      // ระยะเวลาสั่น (วินาที)

    public void ShakeDefault()
    {
        StartCoroutine(ShakeRoutine(defaultDuration, defaultPosMagnitude, defaultRotMagnitude));
    }

    public void Shake(float posMagnitude, float rotMagnitude, float duration = 0.2f)
    {
        StartCoroutine(ShakeRoutine(duration, posMagnitude, rotMagnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float posMag, float rotMag)
    {
        Vector3 originalPos = transform.localPosition;
        Quaternion originalRot = transform.localRotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // 1. สั่นตำแหน่งขนานพื้น (แกน X และ Z)
            float x = Random.Range(-1f, 1f) * posMag;
            float z = Random.Range(-1f, 1f) * posMag;
            transform.localPosition = originalPos + new Vector3(x, 0, z);

            // 2. สั่นหมุนซ้าย-ขวา (แกน Y)
            float yRot = Random.Range(-1f, 1f) * rotMag;
            transform.localRotation = originalRot * Quaternion.Euler(0, yRot, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
        transform.localRotation = originalRot;
    }
}