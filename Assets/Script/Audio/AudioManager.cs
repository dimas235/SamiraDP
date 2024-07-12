using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip[] sfxClips;
    public AudioClip[] bgmClips;

    [Header("Fade In Settings")]
    public float fadeDuration = 2.0f;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    private float bgmStartVolume;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayBGM(0); // Play the first BGM by default

        // Set initial volume levels from PlayerPrefs
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        SetMasterVolume(masterVolume);
        SetBGMVolume(bgmVolume);
        SetSFXVolume(sfxVolume);

    }

    public void StopSFX()
    {
        sfxSource.Stop();
    }

    public void PlayBGM(int index)
    {
        if (index >= 0 && index < bgmClips.Length)
        {
            bgmStartVolume = bgmSource.volume;
            bgmSource.clip = bgmClips[index];
            bgmSource.loop = true;
            bgmSource.Play();
            StartCoroutine(FadeInBGM());
        }
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PlaySFX(int index)
    {
        if (index >= 0 && index < sfxClips.Length)
        {
            sfxSource.PlayOneShot(sfxClips[index]);
        }
    }

    public void SetMasterVolume(float sliderValue)
    {
        float volume = Mathf.Lerp(-30f, 0f, sliderValue / 10f);
        audioMixer.SetFloat("MasterVolume", volume);
        PlayerPrefs.SetFloat("MasterVolume", sliderValue);
    }

    public void SetBGMVolume(float sliderValue)
    {
        float volume = Mathf.Lerp(-30f, 0f, sliderValue / 10f);
        audioMixer.SetFloat("BGMVolume", volume);
        PlayerPrefs.SetFloat("BGMVolume", sliderValue);
    }

    public void SetSFXVolume(float sliderValue)
    {
        float volume = Mathf.Lerp(-30f, 0f, sliderValue / 10f);
        audioMixer.SetFloat("SFXVolume", volume);
        PlayerPrefs.SetFloat("SFXVolume", sliderValue);
    }

    IEnumerator FadeInBGM()
    {
        float currentTime = 0f;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(0, bgmStartVolume, currentTime / fadeDuration);
            yield return null;
        }
    }
}
