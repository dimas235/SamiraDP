using UnityEngine;
using UnityEngine.InputSystem;

public class WaterParticle : Particle
{
    public float effectiveDamage = 10f;

    private ParticleIndicator usageIndicator;
    private bool isSpawning = false; // Menyimpan status apakah partikel sedang dikeluarkan
    private bool isLocked = false; // Menyimpan status apakah partikel terkunci (tidak bisa digunakan)
    private bool wasUsingParticle = false; // Menyimpan status apakah partikel baru saja digunakan

    private void Start()
    {
        // Cari ParticleIndicator di scene
        usageIndicator = FindObjectOfType<ParticleIndicator>();
        if (usageIndicator == null)
        {
            Debug.LogError("ParticleIndicator tidak ditemukan di scene.");
        }
    }

    private void Update()
    {
        // Berhenti mengeluarkan partikel jika indikator penggunaan habis
        if (isSpawning && usageIndicator != null && usageIndicator.CurrentUsage <= 0)
        {
            StopParticle();
            isLocked = true; // Kunci partikel ketika indikator mencapai nol
        }

        // Cek apakah indikator sudah penuh dan buka kunci jika tidak sedang ditekan
        if (isLocked && usageIndicator != null && usageIndicator.CurrentUsage >= usageIndicator.maxUsage && !isSpawning)
        {
            isLocked = false;
        }

        // Pastikan indikator tidak berkurang jika tidak sedang menggunakan partikel
        if (!isSpawning && wasUsingParticle && usageIndicator != null)
        {
            usageIndicator.StopUsingParticle();
            wasUsingParticle = false;
        }
    }

    protected override void OnSpawnStarted(InputAction.CallbackContext context)
    {
        Debug.Log("WaterParticle OnSpawnStarted called");
        if (isLocked)
        {
            // Tidak memulai partikel jika terkunci
            return;
        }

        if (usageIndicator != null && usageIndicator.StartUsingParticle())
        {
            StartParticle();
            wasUsingParticle = true;
        }
    }

    protected override void OnSpawnCanceled(InputAction.CallbackContext context)
    {
        Debug.Log("WaterParticle OnSpawnCanceled called");
        StopParticle();
    }

    private void StartParticle()
    {
        if (!foamParticleSystem.isPlaying)
        {
            foamParticleSystem.Play();
            Debug.Log("WaterParticle Started");
            isSpawning = true;
            TriggerHapticFeedback(true); // Mulai getaran saat partikel mulai
        }
    }

    private void StopParticle()
    {
        if (foamParticleSystem.isPlaying)
        {
            foamParticleSystem.Stop();
            Debug.Log("WaterParticle Stopped");
            isSpawning = false;
            TriggerHapticFeedback(false); // Hentikan getaran saat partikel berhenti
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (!CanApplyDamage())
        {
            return;
        }

        Debug.Log("Particle bertabrakan dengan: " + other.name);

        // Cek apakah objek yang bertabrakan memiliki komponen Fire
        Fire fire = other.GetComponent<Fire>();
        if (fire != null)
        {
            Debug.Log("Komponen Fire terdeteksi pada: " + other.name);

            DamageType damageType = DamageType.None;
            float damageAmount = 0f;

            // Tentukan jenis damage berdasarkan jenis api (fireType)
            switch (fire.fireType)
            {
                case FireType.Organic:
                    damageType = DamageType.Effective;
                    damageAmount = effectiveDamage;
                    break;
                default:
                    Debug.Log("Tidak ada damage yang diterapkan pada tipe api: " + fire.fireType);
                    break; // Tidak ada damage untuk jenis api lainnya
            }

            fire.CheckDamageability(damageType);

            // Terapkan damage ke objek api
            fire.TakeDamage(damageAmount, damageType);
            Debug.Log("Diterapkan " + damageAmount + " damage ke api tipe " + fire.fireType + ". HP tersisa: " + fire.hp);

            // Periksa jika api telah padam
            UIManager uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
            {
                uiManager.CheckAllFiresExtinguished();
            }
        }
        else
        {
            Debug.Log("Tidak ditemukan komponen Fire pada: " + other.name);
        }
    }
}
