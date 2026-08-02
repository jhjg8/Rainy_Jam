using UnityEngine;
using System.Collections;

public class ObjectShaker : MonoBehaviour
{
    [Header("Target (ลากวัตถุที่ต้องการให้สั่นมาใส่)")]
    public Transform targetTransform; // ถ้าไม่ลาก จะสั่นวัตถุที่แปะสคริปต์นี้ให้อัตโนมัติ

    [Header("Shake Settings (X, Z Position & Y Rotation)")]
    public float defaultPosMagnitude = 0.15f; // ความแรงตำแหน่ง (X, Z)
    public float defaultRotMagnitude = 2.0f;  // ความแรงมุมหมุน (Y)
    public float defaultDuration = 0.2f;      // ระยะเวลาสั่น

    private void Awake()
    {
        if (targetTransform == null) targetTransform = transform;
    }

    public void ShakeDefault()
    {
        StartCoroutine(ShakeRoutine(defaultDuration, defaultPosMagnitude, defaultRotMagnitude));
    }

    public void Shake(float posMag, float rotMag, float duration = 0.2f)
    {
        StartCoroutine(ShakeRoutine(duration, posMag, rotMag));
    }

    private IEnumerator ShakeRoutine(float duration, float posMag, float rotMag)
    {
        Vector3 originalPos = targetTransform.localPosition;
        Quaternion originalRot = targetTransform.localRotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // 1. สั่นตำแหน่งขนานพื้น (แกน X และ Z)
            float x = Random.Range(-1f, 1f) * posMag;
            float z = Random.Range(-1f, 1f) * posMag;
            
            // 2. สั่นหมุนรอบแกน Y
            float yRot = Random.Range(-1f, 1f) * rotMag;

            targetTransform.localPosition = originalPos + new Vector3(x, 0, z);
            targetTransform.localRotation = originalRot * Quaternion.Euler(0, yRot, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        targetTransform.localPosition = originalPos;
        targetTransform.localRotation = originalRot;
    }
}