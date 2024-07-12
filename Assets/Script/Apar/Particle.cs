using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using System.Collections;

public class Particle : MonoBehaviour
{
    public ParticleSystem foamParticleSystem;
    public InputActionReference spawnActionRight; // Action dari controller kanan
    public HapticImpulsePlayer hapticImpulsePlayerLeft; // Referensi ke HapticImpulsePlayer di controller kiri

    public float damageInterval = 0.5f; // Interval waktu antara setiap pengurangan HP (dalam detik)
    public float hapticInterval = 0.1f; // Interval untuk getaran haptic

    protected float lastDamageTime; // Waktu terakhir damage diterapkan
    private Coroutine hapticCoroutine; // Coroutine untuk haptic feedback

    private void OnEnable()
    {
        Debug.Log("Particle OnEnable dipanggil");
        spawnActionRight.action.started += OnSpawnStarted;
        spawnActionRight.action.canceled += OnSpawnCanceled;

        if (foamParticleSystem.isPlaying)
        {
            foamParticleSystem.Stop();
        }

        lastDamageTime = Time.time; // Inisialisasi waktu terakhir damage diterapkan
    }

    private void OnDisable()
    {
        Debug.Log("Particle OnDisable dipanggil");
        spawnActionRight.action.started -= OnSpawnStarted;
        spawnActionRight.action.canceled -= OnSpawnCanceled;
    }

    protected virtual void OnSpawnStarted(InputAction.CallbackContext context)
    {
        Debug.Log("OnSpawnStarted dipanggil");
        if (!foamParticleSystem.isPlaying)
        {
            foamParticleSystem.Play();
            Debug.Log("Partikel dimulai");
            TriggerHapticFeedback(true); // Mulai getaran saat partikel mulai
        }
    }

    protected virtual void OnSpawnCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("OnSpawnCanceled dipanggil");
        if (foamParticleSystem.isPlaying)
        {
            foamParticleSystem.Stop();
            Debug.Log("Partikel dihentikan");
            TriggerHapticFeedback(false); // Hentikan getaran saat partikel berhenti
        }
    }

    protected bool CanApplyDamage()
    {
        if (Time.time - lastDamageTime < damageInterval)
        {
            Debug.Log("Damage tidak diterapkan karena belum melewati interval waktu yang ditentukan");
            return false;
        }

        lastDamageTime = Time.time; // Update waktu terakhir damage diterapkan
        return true;
    }

    // Fungsi untuk mengaktifkan atau menghentikan getaran di controller kiri
    protected void TriggerHapticFeedback(bool isPlaying) // Ubah akses ke protected
    {
        Debug.Log("TriggerHapticFeedback dipanggil dengan isPlaying: " + isPlaying);
        if (hapticImpulsePlayerLeft != null)
        {
            Debug.Log("HapticImpulsePlayer ditemukan");
            if (isPlaying)
            {
                if (hapticCoroutine == null)
                {
                    hapticCoroutine = StartCoroutine(HapticFeedbackLoop());
                }
            }
            else
            {
                if (hapticCoroutine != null)
                {
                    StopCoroutine(hapticCoroutine);
                    hapticCoroutine = null;
                }
                hapticImpulsePlayerLeft.SendHapticImpulse(0, 0.1f); // Mengirim getaran dengan amplitudo 0 untuk menghentikan getaran
                Debug.Log("Haptic feedback dihentikan");
            }
        }
        else
        {
            Debug.LogWarning("HapticImpulsePlayer tidak ditemukan!");
        }
    }

    private IEnumerator HapticFeedbackLoop()
    {
        while (true)
        {
            bool result = hapticImpulsePlayerLeft.SendHapticImpulse(1f, hapticInterval);
            Debug.Log(result ? "Haptic feedback berhasil dikirim" : "Gagal mengirim haptic feedback");
            yield return new WaitForSeconds(hapticInterval);
        }
    }
}