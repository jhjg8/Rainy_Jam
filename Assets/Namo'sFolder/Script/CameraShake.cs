using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    [Header("Position Shake (X, Y)")]
    public float defaultPosMagnitude = 0.15f; // ความแรงสั่นตำแหน่ง

    [Header("Rotation Shake (Z Angle)")]
    public float defaultRotMagnitude = 2.0f;  // ความแรงสั่นมุมหมุน

    [Header("Duration")]
    public float defaultDuration = 0.2f;      // ระยะเวลาสั่น (วินาที)

    // สั่งให้สั่นโดยใช้ค่า Default ใน Inspector
    public void ShakeDefault()
    {
        StartCoroutine(ShakeRoutine(defaultDuration, defaultPosMagnitude, defaultRotMagnitude));
    }

    // สั่งให้สั่นโดยกำหนดค่าความแรงเองตามต้องการ
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
            // 1. สั่นตำแหน่ง (X, Y)
            float x = Random.Range(-1f, 1f) * posMag;
            float y = Random.Range(-1f, 1f) * posMag;
            transform.localPosition = originalPos + new Vector3(x, y, 0);

            // 2. สั่นมุมหมุน (Rotation Z)
            float zRot = Random.Range(-1f, 1f) * rotMag;
            transform.localRotation = originalRot * Quaternion.Euler(0, 0, zRot);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // คืนค่าตำแหน่งและมุมหมุนกลับจุดเดิมเสมอ
        transform.localPosition = originalPos;
        transform.localRotation = originalRot;
    }
}