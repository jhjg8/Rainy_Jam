using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // 1. กำหนด Instance เพื่อทำเป็น Singleton ให้สคริปต์อื่นเรียกใช้ได้ง่ายๆ
    public static SoundManager Instance { get; private set; }

    [Header("Audio Clips")]
    [Tooltip("ใส่ไฟล์เสียงต่างๆ ลงในช่องนี้")]
    [SerializeField] private AudioClip[] sfxClips;
    public AudioSource Bgm;
    private void Awake()
    {
        // 2. ตรวจสอบการเป็น Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // ถ้ามี Manager ซ้ำ ให้ลบตัวที่เพิ่งเกิดใหม่ทิ้ง
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // ป้องกันไม่ให้ Manager ถูกทำลายเมื่อเปลี่ยน Scene (ตัวเลือกเสริม)
    }

    /// <summary>
    /// เล่นเสียงจาก Array ตาม Index ณ ตำแหน่ง (Position) ที่ระบุ
    /// </summary>
    /// <param name="clipIndex">ลำดับ Index ในอาเรย์ sfxClips</param>
    /// <param name="position">ตำแหน่ง Vector3 ในฉากที่ต้องการให้เสียงดังขึ้นมา</param>
    /// <param name="volume">ความดังของเสียง (0.0 ถึง 1.0)</param>
    public void PlaySoundAtPosition(int clipIndex, Vector3 position, float volume = 1f)
    {
        // ป้องกัน Error กรณีที่ใส่ Index เกินจำนวนที่มี
        if (sfxClips == null || sfxClips.Length == 0)
        {
            Debug.LogWarning("SoundManager: ยังไม่ได้ใส่ไฟล์เสียงในอาเรย์ sfxClips เลย!");
            return;
        }

        if (clipIndex < 0 || clipIndex >= sfxClips.Length)
        {
            Debug.LogWarning($"SoundManager: หาเสียง Index ที่ {clipIndex} ไม่เจอ (อ้างอิงเกินขอบเขต)");
            return;
        }

        AudioClip clipToPlay = sfxClips[clipIndex];

        if (clipToPlay != null)
        {
            // ใช้คำสั่งนี้เพื่อสร้างเสียงขึ้นมาเล่น ณ ตำแหน่งที่ระบุ
            AudioSource.PlayClipAtPoint(clipToPlay, position, volume);
        }
    }
    public void ChangeBgm(int clipIndex)
    {
        // ป้องกัน Error กรณีที่ใส่ Index เกินจำนวนที่มี
        if (sfxClips == null || sfxClips.Length == 0)
        {
            Debug.LogWarning("SoundManager: ยังไม่ได้ใส่ไฟล์เสียงในอาเรย์ sfxClips เลย!");
            return;
        }

        if (clipIndex < 0 || clipIndex >= sfxClips.Length)
        {
            Debug.LogWarning($"SoundManager: หาเสียง Index ที่ {clipIndex} ไม่เจอ (อ้างอิงเกินขอบเขต)");
            return;
        }

        AudioClip clipToPlay = sfxClips[clipIndex];

        if (clipToPlay != null)
        {
            // ใช้คำสั่งนี้เพื่อสร้างเสียงขึ้นมาเล่น ณ ตำแหน่งที่ระบุ
            Bgm.clip = clipToPlay;
        }
    }
}