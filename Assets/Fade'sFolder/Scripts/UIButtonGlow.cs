using UnityEngine;
using UnityEngine.UI;          // สำหรับ UI พื้นฐานของ Unity
using UnityEngine.EventSystems; // สำหรับการจับ Event เมาส์เข้า-ออก (PointerEnter/Exit)
using DG.Tweening;              // เรียกใช้ DOTween

public class UIButtonGlow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Target Graphic (Text หรือ Image)")]
    [Tooltip("ลาก Text หรือ ข้อความปุ่มมาใส่ตรงนี้ (ถ้าวางว่างไว้ ระบบจะดึงตัวมันเองมาใช้)")]
    public Graphic targetGraphic;

    [Header("Animation Settings")]
    [Tooltip("ระยะเวลาในการเปลี่ยนความสว่าง (วินาที)")]
    public float fadeDuration = 0.2f;

    [Header("Mode A: Brightness & Color (ความสว่างสี)")]
    [Tooltip("สีปกติเมื่อไม่ได้เอาเมาส์จ่อ")]
    public Color normalColor = new Color(0.8f, 0.8f, 0.8f, 1f); // ขาวเทาหม่น

    [Tooltip("สีเมื่อเอาเมาส์จ่อ (สว่างขึ้น)")]
    public Color hoverColor = new Color(1f, 1f, 1f, 1f);      // ขาวสว่างจ้า

    [Header("Mode B: Scale Pop (ขยายขนาดเบาๆ เพิ่มมิติเรืองแสง)")]
    [Tooltip("เปิดใช้งานการขยายขนาดปุ่มเล็กน้อยเมื่อเมาส์จ่อ")]
    public bool enableScaleEffect = true;

    [Tooltip("ขนาดตอนเอาเมาส์จ่อ (1 = ขนาดปกติ, 1.05 = ขยายขึ้น 5%)")]
    public Vector3 hoverScale = new Vector3(1.05f, 1.05f, 1.05f);

    // ตัวแปรเก็บค่าภายใน
    private Vector3 originalScale;
    private Tween colorTween;
    private Tween scaleTween;

    void Awake()
    {
        // 1. ถ้าไม่ได้ลาก targetGraphic มาใส่ ให้หา Component ที่เป็น Text/Image ในตัวเองหรือลูก
        if (targetGraphic == null)
        {
            targetGraphic = GetComponentInChildren<Graphic>();
        }

        // 2. บันทึกขนาดเดิมของวัตถุไว้
        originalScale = transform.localScale;

        // 3. ตั้งค่าสีเริ่มต้น
        if (targetGraphic != null)
        {
            targetGraphic.color = normalColor;
        }
    }

    // ทำงานเมื่อเมาส์ "เลื่อนเข้ามา" บนปุ่ม
    public void OnPointerEnter(PointerEventData eventData)
    {
        KillTweens(); // เคลียร์ Animation เก่าที่ค้างอยู่

        // 1. ปรับสีให้สว่างขึ้นด้วย DOTween
        if (targetGraphic != null)
        {
            colorTween = targetGraphic.DOColor(hoverColor, fadeDuration);
        }

        // 2. ขยายขนาดปุ่มเบาๆ (ถ้าเปิดไว้)
        if (enableScaleEffect)
        {
            scaleTween = transform.DOScale(hoverScale, fadeDuration).SetEase(Ease.OutQuad);
        }
    }

    // ทำงานเมื่อเมาส์ "เลื่อนออก" จากปุ่ม
    public void OnPointerExit(PointerEventData eventData)
    {
        KillTweens(); // เคลียร์ Animation เก่า

        // 1. คืนค่าสีกลับเป็นปกติ
        if (targetGraphic != null)
        {
            colorTween = targetGraphic.DOColor(normalColor, fadeDuration);
        }

        // 2. หดขนาดกลับเท่าเดิม
        if (enableScaleEffect)
        {
            scaleTween = transform.DOScale(originalScale, fadeDuration).SetEase(Ease.OutQuad);
        }
    }

    // ฟังก์ชั่นหยุด Tween เก่าเพื่อไม่ให้ขัดกัน
    private void KillTweens()
    {
        if (colorTween != null && colorTween.IsActive()) colorTween.Kill();
        if (scaleTween != null && scaleTween.IsActive()) scaleTween.Kill();
    }

    void OnDisable()
    {
        KillTweens();
    }
}