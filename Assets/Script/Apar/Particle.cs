using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

public class Particle : MonoBehaviour
{
    public ParticleSystem foamParticleSystem;
    public InputActionReference spawnAction;
    public HapticImpulsePlayer hapticImpulsePlayer; // Tambahkan referensi ke HapticImpulsePlayer

    public float damageInterval = 0.5f; // Interval waktu antara setiap pengurangan HP (dalam detik)

    protected float lastDamageTime; // Waktu terakhir damage diterapkan

    private void OnEnable()
    {
        spawnAction.action.started += OnSpawnStarted;
        spawnAction.action.canceled += OnSpawnCanceled;

        if (foamParticleSystem.isPlaying)
        {
            foamParticleSystem.Stop();
        }

        lastDamageTime = Time.time; // Inisialisasi waktu terakhir damage diterapkan
    }

    private void OnDisable()
    {
        spawnAction.action.started -= OnSpawnStarted;
        spawnAction.action.canceled -= OnSpawnCanceled;
    }

    protected virtual void OnSpawnStarted(InputAction.CallbackContext context)
    {
        if (!foamParticleSystem.isPlaying)
        {
            foamParticleSystem.Play();
            TriggerHapticFeedback(true); // Mulai getaran saat partikel mulai
        }
    }

    protected virtual void OnSpawnCanceled(InputAction.CallbackContext context)
    {
        if (foamParticleSystem.isPlaying)
        {
            foamParticleSystem.Stop();
            TriggerHapticFeedback(false); // Hentikan getaran saat partikel berhenti
        }
    }

    protected bool CanApplyDamage()
    {
        if (Time.time - lastDamageTime < damageInterval)
        {
            // Jika belum melewati interval waktu yang ditentukan, tidak menerapkan damage
            return false;
        }

        lastDamageTime = Time.time; // Update waktu terakhir damage diterapkan
        return true;
    }

    // Fungsi untuk mengaktifkan atau menghentikan getaran di controller kiri
    private void TriggerHapticFeedback(bool isPlaying)
    {
        if (hapticImpulsePlayer != null)
        {
            if (isPlaying)
            {
                hapticImpulsePlayer.SendHapticImpulse(0.5f, 0.1f); // Atur amplitudo dan durasi getaran sesuai keinginan
            }
            else
            {
                // Fungsi untuk menghentikan getaran bisa berbeda tergantung pada implementasi HapticImpulsePlayer Anda
                // Jika tidak ada metode untuk menghentikan getaran, Anda mungkin harus menyesuaikan skrip HapticImpulsePlayer
                hapticImpulsePlayer.SendHapticImpulse(0, 0.1f); // Mengirim getaran dengan amplitudo 0 untuk menghentikan getaran
            }
        }
    }
}
