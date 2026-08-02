using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneTransitionManager : MonoBehaviour
{
    [Header("UI Transition Reference")]
    [Tooltip("ลาก RectTransform ของรูปภาพวงกลมสีดำมาใส่")]
    public RectTransform circleImageRect;

    [Header("Scene Settings")]
    [Tooltip("พิมพ์ชื่อ Scene เกมที่ต้องการโหลดตรงนี้")]
    public string targetSceneName = "GameScene";

    [Header("Transition Settings")]
    [Tooltip("ระยะเวลาที่วงกลมสีดำขยายเต็มจอ (วินาที)")]
    public float expandDuration = 1.0f;

    [Tooltip("ขนาดเป้าหมายของวงกลมตอนขยายเต็มจอ (ปรับเพิ่มได้ถ้าคลุมไม่หมด)")]
    public Vector3 targetScale = new Vector3(30f, 30f, 1f);

    [Header("Audio SFX")]
    [Tooltip("ลาก AudioSource สำหรับเล่นเสียง transition ตอนขยายวงกลม (ถ้ามี)")]
    public AudioSource transitionAudioSource;

    [Tooltip("ไฟล์เสียง Transition (ถ้าไม่มี AudioSource สามารถลากคลิปใส่ตรงนี้ได้)")]
    public AudioClip transitionSFX;

    private bool isTransitioning = false;

    void Awake()
    {
        // ย่อขนาดวงกลมไว้ที่ 0 เสมอตอนโหลดซีน
        if (circleImageRect != null)
        {
            circleImageRect.localScale = Vector3.zero;
        }
    }

    /// <summary>
    /// ฟังก์ชั่นนี้ผูกกับปุ่ม PLAY ใน Main Menu
    /// </summary>
    public void LoadTargetScene()
    {
        // ป้องกันการกดปุ่ม Play ซ้ำซ้อนขณะกำลังเล่น Animation
        if (isTransitioning) return;
        isTransitioning = true;

        // 1. เล่นเสียง SFX Transition
        PlayTransitionSound();

        // 2. เล่น Animation วงกลมขยายจาก 0 ไปตาม targetScale
        if (circleImageRect != null)
        {
            circleImageRect.DOScale(targetScale, expandDuration)
                .SetEase(Ease.InCubic) // จังหวะเริ่มนุ่ม แล้วพุ่งเร็วช่วงท้าย คล้ายชัตเตอร์กล้อง
                .OnComplete(() =>
                {
                    // 3. เมื่อวงกลมขยายคลุมเต็มจอเสร็จสิ้น ให้ทำการเปลี่ยน Scene
                    StartLoadingScene();
                });
        }
        else
        {
            // หากไม่ได้ใส่รูปวงกลม ให้เปลี่ยน Scene ทันทีเพื่อป้องกันเกมค้าง
            StartLoadingScene();
        }
    }

    private void PlayTransitionSound()
    {
        if (transitionAudioSource != null)
        {
            if (transitionSFX != null)
            {
                transitionAudioSource.PlayOneShot(transitionSFX);
            }
            else
            {
                transitionAudioSource.Play();
            }
        }
    }

    private void StartLoadingScene()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            // โหลด Scene ตามชื่อที่พิมพ์ไว้ใน Inspector
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogError("ยังไม่ได้พิมพ์ชื่อ targetSceneName ใน Inspector!");
        }
    }
}