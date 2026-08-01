using UnityEngine;
using UnityEngine.EventSystems; // สำหรับตรวจจับเมาส์ Hover (IPointerEnterHandler)
using UnityEngine.UI;          // สำหรับตรวจจับ Button Click

[RequireComponent(typeof(Button))] // สคริปต์นี้จะบังคับให้ต้องมี Component Button เสมอ
public class UIButtonAudio : MonoBehaviour, IPointerEnterHandler
{
    [Header("Audio Clips")]
    [Tooltip("ไฟล์เสียง SFX ตอนเอาเมาส์มาจ่อที่ปุ่ม")]
    public AudioClip hoverSFX;

    [Tooltip("ไฟล์เสียง SFX ตอนคลิกกดปุ่ม")]
    public AudioClip clickSFX;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    [Tooltip("ความดังเสียงตอน Hover (0 = เงียบ, 1 = ดังสุด)")]
    public float hoverVolume = 0.5f;

    [Range(0f, 1f)]
    [Tooltip("ความดังเสียงตอน คลิกปุ่ม (0 = เงียบ, 1 = ดังสุด)")]
    public float clickVolume = 0.8f;

    [Header("Pitch Randomization (กิมมิกเพิ่มมิติเสียง)")]
    [Tooltip("เปิดการสุ่มระดับเสียง (Pitch) เล็กน้อย เพื่อไม่ให้เสียงฟังดูซ้ำเจจำเจ")]
    public bool enableRandomPitch = true;

    [Range(0.0f, 0.3f)]
    [Tooltip("ช่วงการสุ่ม Pitch (เช่น 0.1 หมายถึง Pitch จะอยู่ในช่วง 0.9 ถึง 1.1)")]
    public float pitchRandomRange = 0.05f;

    // ตัวแปรเก็บค่าภายใน
    private AudioSource audioSource;
    private Button button;

    void Awake()
    {
        // 1. ดึง Component Button ในตัวเอง
        button = GetComponent<Button>();

        // 2. ถ้าในปุ่มยังไม่มี AudioSource ให้ระบบสร้างขึ้นให้อัตโนมัติ
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // ตั้งค่า AudioSource ให้เหมาะกับ UI Sound (เป็นเสียง 2D)
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 0 = 2D Sound (ฟังชัดทั้งสองหู)

        // 3. ผูก Event การกดคลิกปุ่มเข้ากับฟังก์ชั่น เล่นเสียงคลิก
        if (button != null)
        {
            button.onClick.AddListener(PlayClickSFX);
        }
    }

    // ทำงานอัตโนมัติเมื่อเมาส์ "เลื่อนเข้ามาจ่อ" ที่ปุ่ม
    public void OnPointerEnter(PointerEventData eventData)
    {
        // ถ้าปุ่มโดนสั่ง Disable ไว้ (กดไม่ได้) จะไม่เล่นเสียง
        if (button != null && !button.interactable) return;

        if (hoverSFX != null)
        {
            ApplyRandomPitch();
            // PlayOneShot ช่วยให้เสียงเล่นซ้อนกันได้นุ่มนวล แม้เมาส์จะกวาดผ่านปุ่มไวๆ
            audioSource.PlayOneShot(hoverSFX, hoverVolume);
        }
    }

    // ฟังก์ชั่นเล่นเสียงตอน "กดคลิก" ปุ่ม
    public void PlayClickSFX()
    {
        if (clickSFX != null)
        {
            ApplyRandomPitch();
            audioSource.PlayOneShot(clickSFX, clickVolume);
        }
    }

    // ระบบสุ่ม Pitch ให้เสียงมีชีวิตชีวาขึ้น
    private void ApplyRandomPitch()
    {
        if (enableRandomPitch)
        {
            audioSource.pitch = Random.Range(1f - pitchRandomRange, 1f + pitchRandomRange);
        }
        else
        {
            audioSource.pitch = 1f; // ค่าปกติ
        }
    }

    void OnDestroy()
    {
        // เคลียร์ Listener เมื่อ Object ถูกทำลาย
        if (button != null)
        {
            button.onClick.RemoveListener(PlayClickSFX);
        }
    }
}