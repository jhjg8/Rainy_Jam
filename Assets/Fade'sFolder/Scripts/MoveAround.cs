using UnityEngine;
using DG.Tweening;

public class MoveAround : MonoBehaviour
{
    public RectTransform rectTransform;

    [Header("Movement Settings")]
    public float moveRadius = 50f;      // รัศมีสูงสุดที่จะขยับไปได้จากจุดเริ่มต้น
    public float moveSpeed = 1f;       // ความเร็วในการเปลี่ยนทิศทาง (ยิ่งมากยิ่งขยับเร็ว)
    public float smoothness = 0.5f;    // ความนุ่มนวลในการเคลื่อนที่ (ใช้กับ DOTween)
    public float rotationAmount = 100f;   // จำนวนองศาที่จะหมุน (ใช้กับ DOTween)

    private Vector2 originalPos;
    private float noiseSeedX;
    private float noiseSeedY;

    void Start()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        // เก็บตำแหน่งเริ่มต้นไว้เป็นจุดศูนย์กลาง
        originalPos = rectTransform.anchoredPosition;

        // สุ่มค่า Seed เพื่อให้วัตถุแต่ละชิ้นขยับไม่เหมือนกัน
        noiseSeedX = Random.value * 100f;
        noiseSeedY = Random.value * 100f;

    }

    void Update()
    {
        float noiseX = Mathf.PerlinNoise(Time.time * moveSpeed, noiseSeedX) - 0.5f;
        float noiseY = Mathf.PerlinNoise(Time.time * moveSpeed, noiseSeedY) - 0.5f;

        Vector2 targetPos = originalPos + new Vector2(noiseX * 2f * moveRadius, noiseY * 2f * moveRadius);
        Quaternion targetRot = Quaternion.Euler(0f, 0f, noiseX * rotationAmount);

        rectTransform.DOKill();
        rectTransform.DOAnchorPos(targetPos, smoothness).SetEase(Ease.InOutSine);
        rectTransform.DORotate(targetRot.eulerAngles, smoothness).SetEase(Ease.InOutSine);
    }
}
