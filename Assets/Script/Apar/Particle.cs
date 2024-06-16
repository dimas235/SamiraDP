using UnityEngine;
using UnityEngine.InputSystem;

public class Particle : MonoBehaviour
{
    public ParticleSystem foamParticleSystem;
    public InputActionReference spawnAction;

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
        }
    }

    protected virtual void OnSpawnCanceled(InputAction.CallbackContext context)
    {
        if (foamParticleSystem.isPlaying)
        {
            foamParticleSystem.Stop();
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
}
