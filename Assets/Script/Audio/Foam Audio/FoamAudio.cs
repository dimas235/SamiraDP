using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Audio;

public class FoamAudio : MonoBehaviour
{
    public InputActionReference input;
    public AudioClip pressSFX;        // Audio clip untuk SFX saat ditekan
    public AudioClip releaseSFX;      // Audio clip untuk SFX saat dilepas
    public AudioClip sprayLoopSFX;    // Audio clip untuk suara semprot
    public AudioClip stopSpraySFX;    // Audio clip untuk SFX saat berhenti semprot
    public AudioMixerGroup sfxMixerGroup; // Audio Mixer Group untuk SFX

    private AudioSource pressAudioSource;
    private AudioSource releaseAudioSource;
    private AudioSource sprayAudioSource;
    private AudioSource stopSprayAudioSource;
    private bool isSpraying = false;

    // Start is called before the first frame update
    void Start()
    {
        pressAudioSource = gameObject.AddComponent<AudioSource>();
        releaseAudioSource = gameObject.AddComponent<AudioSource>();
        sprayAudioSource = gameObject.AddComponent<AudioSource>();
        stopSprayAudioSource = gameObject.AddComponent<AudioSource>();

        pressAudioSource.clip = pressSFX;
        releaseAudioSource.clip = releaseSFX;
        sprayAudioSource.clip = sprayLoopSFX;
        sprayAudioSource.loop = true;
        stopSprayAudioSource.clip = stopSpraySFX;

        // Set output audio mixer group for all audio sources
        pressAudioSource.outputAudioMixerGroup = sfxMixerGroup;
        releaseAudioSource.outputAudioMixerGroup = sfxMixerGroup;
        sprayAudioSource.outputAudioMixerGroup = sfxMixerGroup;
        stopSprayAudioSource.outputAudioMixerGroup = sfxMixerGroup;

        input.action.started += OnInputStarted;
        input.action.canceled += OnInputCanceled;
    }

    void OnInputStarted(InputAction.CallbackContext context)
    {
        PlayPressSFX();
        StartSpraying();
    }

    void OnInputCanceled(InputAction.CallbackContext context)
    {
        PlayReleaseSFX();
        StopSpraying();
        PlayStopSpraySFX();
    }

    public void PlayPressSFX()
    {
        if (pressAudioSource != null && pressSFX != null)
        {
            pressAudioSource.Play();
        }
    }

    public void PlayReleaseSFX()
    {
        if (releaseAudioSource != null && releaseSFX != null)
        {
            releaseAudioSource.Play();
        }
    }

    public void StartSpraying()
    {
        if (sprayAudioSource != null && sprayLoopSFX != null)
        {
            isSpraying = true;
            sprayAudioSource.Play();
        }
    }

    public void StopSpraying()
    {
        if (sprayAudioSource != null && isSpraying)
        {
            isSpraying = false;
            sprayAudioSource.Stop();
        }
    }

    public void PlayStopSpraySFX()
    {
        if (stopSprayAudioSource != null && stopSpraySFX != null)
        {
            stopSprayAudioSource.Play();
        }
    }

    private void OnDestroy()
    {
        input.action.started -= OnInputStarted;
        input.action.canceled -= OnInputCanceled;
    }
}
