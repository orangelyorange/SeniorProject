using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music Clips")]
    public AudioClip mainMenuMusic;
    public AudioClip level1Music;
    public AudioClip level2Music;
    public AudioClip level3Music;

    [Header("Player SFX")]
    public AudioClip playerJump;
    public AudioClip playerLand;
    public AudioClip playerAttack;
    public AudioClip playerPlungeSlam;
    public AudioClip playerDash;
    public AudioClip playerTakeDamage;
    public AudioClip playerDeath;
    public AudioClip playerHeal;

    [Header("Emotion Skill SFX")]
    public AudioClip joyActivate;
    public AudioClip sadnessActivate;
    public AudioClip sadnessShieldExpire;
    public AudioClip rageActivate;

    [Header("Enemy SFX")]
    public AudioClip enemyTakeDamage;
    public AudioClip enemyDie;
    public AudioClip enemyShoot;

    [Header("Item SFX")]
    public AudioClip itemPickup;
    public AudioClip itemDrop;

    [Header("Environment SFX")]
    public AudioClip jumpPad;
    public AudioClip fallingPlatformCrumble;

    [Header("UI SFX")]
    public AudioClip buttonClick;
    public AudioClip questComplete;
    public AudioClip panelOpen;

    [Header("Volume Settings")]
    [Range(0f, 1f)] [SerializeField] private float masterVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float musicVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;

    [Header("Fade Settings")]
    [SerializeField] private float defaultFadeDuration = 1f;

    public float MasterVolume => masterVolume;
    public float MusicVolume => musicVolume;
    public float SfxVolume => sfxVolume;

    private const string MasterVolumeKey = "MasterVolume";
    private const string MusicVolumeKey = "MusicVolume";
    private const string SfxVolumeKey = "SfxVolume";

    private Coroutine musicFadeCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        EnsureAudioSources();
        LoadVolumeSettings();
        ApplyVolumes();
    }

    private void EnsureAudioSources()
    {
        if (musicSource == null)
        {
            musicSource = GetComponent<AudioSource>();
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
            }
        }

        if (sfxSource == null)
        {
            AudioSource[] sources = GetComponents<AudioSource>();
            if (sources.Length > 1)
            {
                sfxSource = sources[1];
            }
            else
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
            }
        }

        musicSource.loop = true;
        musicSource.playOnAwake = false;

        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
    }

    private void LoadVolumeSettings()
    {
        masterVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(MasterVolumeKey, 1f));
        musicVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(MusicVolumeKey, 1f));
        sfxVolume = Mathf.Clamp01(PlayerPrefs.GetFloat(SfxVolumeKey, 1f));
    }

    private void ApplyVolumes()
    {
        if (musicSource != null)
        {
            musicSource.volume = masterVolume * musicVolume;
        }

        if (sfxSource != null)
        {
            sfxSource.volume = 1f;
        }
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(MasterVolumeKey, masterVolume);
        PlayerPrefs.Save();
        ApplyVolumes();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
        PlayerPrefs.Save();
        ApplyVolumes();
    }

    public void SetSfxVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(SfxVolumeKey, sfxVolume);
        PlayerPrefs.Save();
        ApplyVolumes();
    }

    /// <summary>
    /// Plays a music clip and optionally crossfades from the currently active track.
    /// If the same clip is already playing, this call does nothing.
    /// </summary>
    /// <param name="clip">Music clip to play.</param>
    /// <param name="fadeIn">When true, performs a crossfade/fade-in transition.</param>
    public void PlayMusic(AudioClip clip, bool fadeIn = true)
    {
        if (clip == null || musicSource == null)
        {
            return;
        }

        if (musicSource.clip == clip && musicSource.isPlaying)
        {
            return;
        }

        if (musicFadeCoroutine != null)
        {
            StopCoroutine(musicFadeCoroutine);
        }

        if (!fadeIn)
        {
            musicSource.clip = clip;
            musicSource.volume = masterVolume * musicVolume;
            musicSource.Play();
            return;
        }

        musicFadeCoroutine = StartCoroutine(CrossfadeMusic(clip));
    }

    public void StopMusic(bool fadeOut = true)
    {
        if (musicSource == null)
        {
            return;
        }

        if (!fadeOut)
        {
            if (musicFadeCoroutine != null)
            {
                StopCoroutine(musicFadeCoroutine);
                musicFadeCoroutine = null;
            }

            musicSource.Stop();
            return;
        }

        FadeOutMusic(defaultFadeDuration);
    }

    public Coroutine FadeOutMusic(float duration)
    {
        if (musicSource == null)
        {
            return null;
        }

        if (musicFadeCoroutine != null)
        {
            StopCoroutine(musicFadeCoroutine);
        }

        musicFadeCoroutine = StartCoroutine(FadeOutAndStop(duration));
        return musicFadeCoroutine;
    }

    public void PlaySfx(AudioClip clip)
    {
        if (clip == null || sfxSource == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip, masterVolume * sfxVolume);
    }

    private IEnumerator CrossfadeMusic(AudioClip nextClip)
    {
        float targetVolume = masterVolume * musicVolume;

        if (musicSource.isPlaying)
        {
            float startVolume = musicSource.volume;
            float time = 0f;

            while (time < defaultFadeDuration)
            {
                time += Time.unscaledDeltaTime;
                musicSource.volume = Mathf.Lerp(startVolume, 0f, time / defaultFadeDuration);
                yield return null;
            }
        }

        musicSource.Stop();
        musicSource.clip = nextClip;
        musicSource.volume = 0f;
        musicSource.Play();

        float fadeInTime = 0f;
        while (fadeInTime < defaultFadeDuration)
        {
            fadeInTime += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(0f, targetVolume, fadeInTime / defaultFadeDuration);
            yield return null;
        }

        musicSource.volume = targetVolume;
        musicFadeCoroutine = null;
    }

    private IEnumerator FadeOutAndStop(float duration)
    {
        if (!musicSource.isPlaying)
        {
            musicFadeCoroutine = null;
            yield break;
        }

        float startVolume = musicSource.volume;
        float time = 0f;
        float fadeDuration = Mathf.Max(0.01f, duration);

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, time / fadeDuration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = masterVolume * musicVolume;
        musicFadeCoroutine = null;
    }
}
