using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioSource pitchSfxSource;

    [Header("BGM Clips")]
    public AudioClip menuBGM;
    public AudioClip gameBGM;
    public AudioClip shopBGM; // เพิ่ม BGM ร้านค้า

    [Header("SFX Clips (Interact & Actions)")]
    public AudioClip interactCardSFX;
    public AudioClip attackSFX;
    public AudioClip defenseSFX;
    public AudioClip damageSFX;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMenuBGM();
    }

    // ==========================================
    // 1. ระบบ BGM (3 หน้า)
    // ==========================================

    public void PlayMenuBGM() => PlayBGM(menuBGM);
    public void PlayGameBGM() => PlayBGM(gameBGM);
    public void PlayShopBGM() => PlayBGM(shopBGM); // ฟังก์ชันสลับไป BGM ร้านค้า

    private void PlayBGM(AudioClip clip)
    {
        if (clip == null || bgmSource == null) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource != null) bgmSource.Stop();
    }

    // ==========================================
    // 2. ระบบ SFX (Random Pitch)
    // ==========================================

    public void PlayRandomPitchSFX(AudioClip clip, float minPitch = 0.9f, float maxPitch = 1.1f)
    {
        if (clip == null || pitchSfxSource == null) return;

        pitchSfxSource.pitch = Random.Range(minPitch, maxPitch);
        pitchSfxSource.PlayOneShot(clip);
    }

    public void PlayNormalSFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;

        sfxSource.pitch = 1f;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayInteractCardSFX() => PlayRandomPitchSFX(interactCardSFX);
    public void PlayAttackSFX()       => PlayRandomPitchSFX(attackSFX);
    public void PlayDefenseSFX()      => PlayRandomPitchSFX(defenseSFX);
    public void PlayDamageSFX()       => PlayRandomPitchSFX(damageSFX);
}