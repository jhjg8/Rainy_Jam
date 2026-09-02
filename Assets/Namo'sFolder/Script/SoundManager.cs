using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioSource pitchSfxSource;

    [Header("BGM Clips")]
    public AudioClip menuBGM;
    public AudioClip gameBGM;
    public AudioClip shopBGM;

    [Header("SFX Clips (Interact & Actions)")]
    public AudioClip interactCardSFX;
    public AudioClip attackSFX;
    public AudioClip defenseSFX;
    public AudioClip damageSFX;

    [Header("BGM Settings")]
    [Range(0f, 1f)] public float bgmVolume = 0.8f;
    public float bgmFadeDuration = 0.5f;

    private Coroutine bgmFadeRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (bgmSource == null) bgmSource = GetComponent<AudioSource>();
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
        }
        if (sfxSource == null)
        {
            var go = new GameObject("SFX_Source");
            go.transform.SetParent(transform);
            sfxSource = go.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }
        if (pitchSfxSource == null)
        {
            var go = new GameObject("PitchSFX_Source");
            go.transform.SetParent(transform);
            pitchSfxSource = go.AddComponent<AudioSource>();
            pitchSfxSource.playOnAwake = false;
        }
    }

    private void Start()
    {
        PlayMenuBGM();
    }

    public void PlayMenuBGM() => PlayBGM(menuBGM);
    public void PlayGameBGM() => PlayBGM(gameBGM);
    public void PlayShopBGM() => PlayBGM(shopBGM);

    private void PlayBGM(AudioClip clip)
    {
        if (clip == null || bgmSource == null) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        if (bgmFadeRoutine != null) StopCoroutine(bgmFadeRoutine);
        bgmFadeRoutine = StartCoroutine(FadeToBGM(clip));
    }

    private IEnumerator FadeToBGM(AudioClip newClip)
    {
        float startVol = bgmSource.volume;
        if (bgmSource.isPlaying)
        {
            float t = 0f;
            while (t < bgmFadeDuration * 0.5f)
            {
                t += Time.deltaTime;
                bgmSource.volume = Mathf.Lerp(startVol, 0f, t / (bgmFadeDuration * 0.5f));
                yield return null;
            }
            bgmSource.Stop();
        }

        bgmSource.clip = newClip;
        bgmSource.loop = true;
        bgmSource.volume = 0f;
        bgmSource.Play();

        float t2 = 0f;
        while (t2 < bgmFadeDuration * 0.5f)
        {
            t2 += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0f, bgmVolume, t2 / (bgmFadeDuration * 0.5f));
            yield return null;
        }
        bgmSource.volume = bgmVolume;
        bgmFadeRoutine = null;
    }

    public void StopBGM()
    {
        if (bgmFadeRoutine != null) StopCoroutine(bgmFadeRoutine);
        if (bgmSource != null) bgmSource.Stop();
    }

    public void SetBGMVolume(float vol)
    {
        bgmVolume = Mathf.Clamp01(vol);
        if (bgmSource != null) bgmSource.volume = bgmVolume;
    }

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
