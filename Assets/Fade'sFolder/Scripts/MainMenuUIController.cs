using UnityEngine;
using DG.Tweening; // ใช้ DOTween สำหรับสั่ง Fade BGM นุ่มนวล

public class MainMenuUIController : MonoBehaviour
{
    [Header("UI Canvas & Rect")]
    public CanvasGroup mainUICanvasGroup;
    public RectTransform uiPanelRect;

    [Header("Audio Settings")]
    [Tooltip("ลาก AudioSource ของ BGM มาใส่ตรงนี้")]
    public AudioSource bgmAudioSource;

    [Tooltip("เปิดใช้งาน Fade In เสียงเพลงให้นุ่มนวลหรือไม่")]
    public bool useFadeInBGM = true;

    [Tooltip("ระยะเวลาการ Fade In เสียง BGM (วินาที)")]
    public float bgmFadeDuration = 1.5f;

    [Header("Slide Animation Settings")]
    public float startOffsetX = -1000f;
    public float moveDuration = 0.8f;
    public float overshootAmount = 1.2f;

    private Vector2 targetAnchoredPosition;
    private float maxBGMVolume = 1f; // จำค่าความดังสูงสุดเดิมไว้

    void Awake()
    {
        if (uiPanelRect != null)
        {
            targetAnchoredPosition = uiPanelRect.anchoredPosition;
            uiPanelRect.anchoredPosition = new Vector2(targetAnchoredPosition.x + startOffsetX, targetAnchoredPosition.y);
        }

        if (mainUICanvasGroup != null)
        {
            mainUICanvasGroup.alpha = 0f;
            mainUICanvasGroup.blocksRaycasts = false;
            mainUICanvasGroup.interactable = false;
        }

        // เตรียมความพร้อมของ BGM ก่อนเริ่ม
        if (bgmAudioSource != null)
        {
            maxBGMVolume = bgmAudioSource.volume; // เซฟค่า Volume ที่ตั้งไว้ใน Inspector

            if (useFadeInBGM)
            {
                bgmAudioSource.volume = 0f; // ตั้งระดับเสียงเริ่มที่ 0 ก่อน
            }
        }
    }

    /// <summary>
    /// ฟังก์ชั่นนี้ถูกเรียกโดย Signal Emitter เมื่อ Cutscene เล่นจบ
    /// </summary>
    public void OnCutsceneFinished()
    {
        Debug.Log("Cutscene Finished -> Sliding UI & Playing BGM!");

        // 1. จัดการ UI
        if (mainUICanvasGroup != null)
        {
            mainUICanvasGroup.alpha = 1f;
            mainUICanvasGroup.blocksRaycasts = true;
            mainUICanvasGroup.interactable = true;
        }

        // 2. สั่ง UI เลื่อนพร้อม Overshoot
        if (uiPanelRect != null)
        {
            uiPanelRect.DOAnchorPos(targetAnchoredPosition, moveDuration)
                .SetEase(Ease.OutBack, overshootAmount);
        }

        // 3. สั่งเล่นเพลง BGM ให้เข้าจังหวะกับ UI
        PlayBGM();
    }

    private void PlayBGM()
    {
        if (bgmAudioSource == null) return;

        bgmAudioSource.Play(); // สั่งเล่นเพลง

        if (useFadeInBGM)
        {
            // ใช้ DOTween ค่อย ๆ ดันเสียงขึ้นจาก 0 ถึงความดังเดิม
            bgmAudioSource.DOFade(maxBGMVolume, bgmFadeDuration);
        }
    }
}