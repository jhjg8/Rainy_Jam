using UnityEngine;
using DG.Tweening; // เรียกใช้งาน DOTween Library

public class UIMouseParallax : MonoBehaviour
{
    [Header("Parallax Settings")]
    [Tooltip("ระยะการขยับสูงสุดจากจุดเริ่มต้น (หน่วยเป็น Pixels)")]
    public float maxMovingRadius = 30f;

    [Tooltip("ความนุ่มนวลในการเคลื่อนที่ (ยิ่งค่าน้อยยิ่งตอบสนองไว ยิ่งมากยิ่งสไลด์นุ่ม)")]
    public float moveDuration = 0.3f;

    [Tooltip("ติ๊กถูกเพื่อเปลี่ยนทิศทาง ให้ UI ขยับสวนทางกับเมาส์ (สร้างมิติความลึก)")]
    public bool invertDirection = false;

    // ตัวแปรเก็บค่าภายในระบบ
    private RectTransform rectTransform; // ตัวเก็บตำแหน่งของ UI
    private Vector2 initialPosition;    // ตำแหน่งเริ่มต้นแท้จริงของ UI ชิ้นนี้
    private Vector2 targetPosition;     // ตำแหน่งเป้าหมายใหม่ที่ต้องการให้ขยับไป
    private Tween moveTween;            // ตัวเก็บตัวควบคุมการเคลื่อนที่ของ DOTween

    void Awake()
    {
        // ดึง RectTransform ของ GameObject ที่แปะ Script นี้อยู่มาใช้งาน
        rectTransform = GetComponent<RectTransform>();
        
        // บันทึกตำแหน่งเริ่มต้น (Original Position) เอาไว้เป็นจุดศูนย์กลาง
        initialPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        CalculateMouseParallax();
    }

    private void CalculateMouseParallax()
    {
        // 1. หาตำแหน่งจุดกึ่งกลางของหน้าจอ (Screen Center)
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);

        // 2. คำนวณหาระยะห่างจากเมาส์ ถึง จุดกึ่งกลางหน้าจอ
        Vector2 mouseOffset = (Vector2)Input.mousePosition - screenCenter;

        // 3. แปลงระยะห่างให้กลายเป็นค่าสัดส่วน Ratio ระหว่าง -1 ถึง 1
        // (ขอบซ้ายสุด/ล่างสุด = -1, ตรงกลาง = 0, ขอบขวาสุด/บนสุด = 1)
        Vector2 normalizedOffset = new Vector2(
            mouseOffset.x / screenCenter.x,
            mouseOffset.y / screenCenter.y
        );

        // 4. จำกัดค่าไม่ให้เกิน -1 ถึง 1 กรณีเมาส์หลุดออกนอกขอบจอไปนิดหน่อย
        normalizedOffset.x = Mathf.Clamp(normalizedOffset.x, -1f, 1f);
        normalizedOffset.y = Mathf.Clamp(normalizedOffset.y, -1f, 1f);

        // 5. ถ้าติ๊ก Invert ให้สลับทิศทาง (คูณด้วย -1)
        if (invertDirection)
        {
            normalizedOffset *= -1f;
        }

        // 6. คำนวณหาตำแหน่งเป้าหมายใหม่ โดยอิงจากจุดเริ่มต้น + (ทิศทาง * ระยะรัศมีสูงสุด)
        targetPosition = initialPosition + (normalizedOffset * maxMovingRadius);

        // 7. ใช้ DOTween ในการย้าย UI ไปยังตำแหน่งเป้าหมาย
        // ถ้ามี Tween เก่าที่กำลังทำงานอยู่ ให้ยกเลิกค่าเก่าก่อน (เพื่อไม่ให้ขยับกระตุกหรือตีกัน)
        if (moveTween != null && moveTween.IsActive())
        {
            moveTween.Kill();
        }

        // สั่งให้ขยับไปยัง targetPosition ด้วยความนุ่มนวลแบบ OutQuad
        moveTween = rectTransform.DOAnchorPos(targetPosition, moveDuration)
            .SetEase(Ease.OutQuad);
    }

    void OnDisable()
    {
        // เคลียร์ Tween เมื่อ Script ถูกสั่งปิด เพื่อป้องกัน Memory Leak
        if (moveTween != null && moveTween.IsActive())
        {
            moveTween.Kill();
        }
    }
}