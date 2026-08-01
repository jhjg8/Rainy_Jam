using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class UIPopupWindow : MonoBehaviour
{
    [Header("UI Components")]
    [Tooltip("Panel หน้าต่างที่จะให้เลื่อนลงมา (ถ้าปล่อยว่าง จะใช้ RectTransform ของวัตถุนี้)")]
    public RectTransform windowRect;

    [Tooltip("CanvasGroup ของพื้นหลังสีดำใส (Dim Background) สำหรับทำ Fade")]
    public CanvasGroup dimBackgroundCanvasGroup;

    [Header("Animation Settings")]
    [Tooltip("ระยะที่หน้าต่างถอยไปซ่อนด้านบน (ค่าบวก เช่น 1200)")]
    public float topOffsetY = 1200f;

    [Tooltip("ระยะเวลาเคลื่อนที่ (วินาที)")]
    public float duration = 0.6f;

    [Tooltip("ความเด้ง Overshoot ตอนเปิด (1.0 - 1.5)")]
    public float openOvershoot = 1.2f;

    [Header("Options")]
    [Tooltip("เปิดหน้าต่างนี้ทันทีเมื่อเริ่มเกมหรือไม่?")]
    public bool openOnStart = false;

    private CanvasGroup windowCanvasGroup;
    private Vector2 targetAnchoredPos;
    private Tween activeMoveTween;
    private Tween activeFadeTween;
    private bool isInitialized = false;

    void Awake()
    {
        // บันทึกตำแหน่งตั้งต้นทันทีที่โหลดซีน
        EnsureInitialized();
    }

    void Start()
    {
        if (openOnStart)
        {
            OpenWindow();
        }
        else
        {
            // ถ้าไม่ใช่หน้าต่างที่เปิดตอนเริ่ม ให้ซ่อนตัวรอไว้ทันที
            SetHiddenStateImmediate();
        }
    }

    /// <summary>
    /// บันทึกค่าตำแหน่งและส่วนประกอบต่างๆ ให้เรียบร้อย
    /// </summary>
    private void EnsureInitialized()
    {
        if (isInitialized) return;

        windowCanvasGroup = GetComponent<CanvasGroup>();
        if (windowRect == null) windowRect = GetComponent<RectTransform>();

        // เก็บค่าตำแหน่งที่คุณจัดไว้ใน Canvas เป็นตำแหน่งปลายทาง
        targetAnchoredPos = windowRect.anchoredPosition;

        isInitialized = true;
    }

    /// <summary>
    /// ย้ายหน้าต่างไปซ่อนด้านบนและปรับค่า Alpha เป็น 0 ทันทีโดยไม่มี Animation
    /// </summary>
    private void SetHiddenStateImmediate()
    {
        EnsureInitialized();

        // ยกเลิก Tween เก่าถ้ามี
        KillActiveTweens();

        // 1. ดันตำแหน่งขึ้นไปซ่อนขอบบน
        windowRect.anchoredPosition = new Vector2(targetAnchoredPos.x, targetAnchoredPos.y + topOffsetY);

        // 2. ปรับ Alpha เป็น 0 และปิดการกด
        windowCanvasGroup.alpha = 0f;
        windowCanvasGroup.blocksRaycasts = false;
        windowCanvasGroup.interactable = false;

        if (dimBackgroundCanvasGroup != null)
        {
            dimBackgroundCanvasGroup.alpha = 0f;
            dimBackgroundCanvasGroup.blocksRaycasts = false;
            dimBackgroundCanvasGroup.interactable = false;
        }
    }

    /// <summary>
    /// สั่งเปิดหน้าต่าง (สไลด์ลงมาจากบน + Fade In ฉากหลัง)
    /// </summary>
    public void OpenWindow()
    {
        // 1. มั่นใจว่าได้บันทึกค่าตำแหน่งเรียบร้อยแล้ว
        EnsureInitialized();

        // ปลุก GameObject ให้ทำงาน (กรณีบังเอิญถูกปิดไว้)
        if (!gameObject.activeSelf) gameObject.SetActive(true);

        KillActiveTweens();

        // 2. ตั้งค่าตำแหน่งเริ่มต้นให้อยู่ข้างบนก่อนเล่น Animation
        windowRect.anchoredPosition = new Vector2(targetAnchoredPos.x, targetAnchoredPos.y + topOffsetY);

        // 3. เปิดการมองเห็นและการตอบสนอง
        windowCanvasGroup.alpha = 1f;
        windowCanvasGroup.blocksRaycasts = true;
        windowCanvasGroup.interactable = true;

        if (dimBackgroundCanvasGroup != null)
        {
            dimBackgroundCanvasGroup.blocksRaycasts = true;
            dimBackgroundCanvasGroup.interactable = true;
            activeFadeTween = dimBackgroundCanvasGroup.DOFade(1f, duration);
        }

        // 4. สไลด์ลงมาตำแหน่งเป้าหมายพร้อม Overshoot
        activeMoveTween = windowRect.DOAnchorPos(targetAnchoredPos, duration)
            .SetEase(Ease.OutBack, openOvershoot);
    }

    /// <summary>
    /// สั่งปิดหน้าต่าง (สไลด์กลับขึ้นไปด้านบน + Fade Out ฉากหลัง)
    /// </summary>
    public void CloseWindow()
    {
        EnsureInitialized();
        KillActiveTweens();

        // ปิดการตอบสนองคลิกทันที ป้องกันกดซ้ำ
        windowCanvasGroup.blocksRaycasts = false;
        windowCanvasGroup.interactable = false;

        if (dimBackgroundCanvasGroup != null)
        {
            dimBackgroundCanvasGroup.blocksRaycasts = false;
            dimBackgroundCanvasGroup.interactable = false;
            activeFadeTween = dimBackgroundCanvasGroup.DOFade(0f, duration);
        }

        Vector2 hidePos = new Vector2(targetAnchoredPos.x, targetAnchoredPos.y + topOffsetY);

        // สไลด์กลับขึ้นข้างบน
        activeMoveTween = windowRect.DOAnchorPos(hidePos, duration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                // ซ่อน Alpha เมื่อเล่นแอนิเมชันเสร็จ
                windowCanvasGroup.alpha = 0f;
            });
    }

    private void KillActiveTweens()
    {
        if (activeMoveTween != null && activeMoveTween.IsActive()) activeMoveTween.Kill();
        if (activeFadeTween != null && activeFadeTween.IsActive()) activeFadeTween.Kill();
    }
}