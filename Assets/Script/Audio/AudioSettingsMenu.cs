using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsMenu : MonoBehaviour
{
    public Slider masterSlider;
    public Slider inGameMasterSlider;
    public Slider bgmSlider;
    public Slider inGameBGMSlider;
    public Slider sfxSlider;
    public Slider inGameSFXSlider;

    private void Start()
    {
        // Set initial values for sliders from PlayerPrefs
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        inGameMasterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
        inGameBGMSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        inGameSFXSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // Add listeners for sliders
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        inGameMasterSlider.onValueChanged.AddListener(SetMasterVolume);
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        inGameBGMSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        inGameSFXSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    private void SetMasterVolume(float volume)
    {
        AudioManager.instance.SetMasterVolume(volume);
    }

    private void SetBGMVolume(float volume)
    {
        AudioManager.instance.SetBGMVolume(volume);
    }

    private void SetSFXVolume(float volume)
    {
        AudioManager.instance.SetSFXVolume(volume);
    }
}
