using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParticleIndicator : MonoBehaviour
{
    public Image usageBar; // Referensi ke UI Image untuk bar penggunaan
    public float maxUsage = 100f; // Jumlah maksimum penggunaan
    public float usageRate = 10f; // Kecepatan penggunaan ketika partikel digunakan
    public float rechargeRate = 5f; // Kecepatan pengisian ulang ketika partikel tidak digunakan
    public float emptyRechargeRate = 10f; // Kecepatan pengisian ulang ketika bar kosong
    public Color normalColor = Color.green; // Warna normal
    public Color lowColor = Color.red; // Warna ketika penggunaan rendah
    public float lowThreshold = 20f; // Batas penggunaan rendah untuk mengganti warna
    public float blinkDuration = 1f; // Durasi waktu untuk satu siklus fade in dan fade out
    private float currentUsage; // Jumlah penggunaan saat ini

    private bool isUsingParticle = false;
    private bool isBlinking = false; // Menyimpan status apakah bar sedang berkedip

    public float CurrentUsage
    {
        get { return currentUsage; }
    }

    // Start dipanggil sebelum frame update pertama
    void Start()
    {
        currentUsage = maxUsage;
        if (usageBar != null)
        {
            usageBar.fillAmount = currentUsage / maxUsage;
            usageBar.color = normalColor; // Set warna awal
        }
    }

    // Update dipanggil sekali per frame
    void Update()
    {
        if (isUsingParticle)
        {
            // Mengurangi penggunaan saat partikel digunakan
            currentUsage -= usageRate * Time.deltaTime;
            if (currentUsage <= 0)
            {
                currentUsage = 0;
                isUsingParticle = false; // Berhenti menggunakan partikel ketika penggunaan habis
                StartBlinking(); // Mulai efek kedip saat bar habis
            }
        }
        else
        {
            // Mengisi ulang penggunaan ketika partikel tidak digunakan
            if (currentUsage == 0)
            {
                currentUsage += emptyRechargeRate * Time.deltaTime;
            }
            else
            {
                currentUsage += rechargeRate * Time.deltaTime;
            }

            if (currentUsage > maxUsage)
            {
                currentUsage = maxUsage;
                StopBlinking(); // Hentikan efek kedip saat bar penuh
            }
        }

        // Memperbarui fill amount dan warna dari usage bar
        if (usageBar != null)
        {
            usageBar.fillAmount = currentUsage / maxUsage;

            if (currentUsage <= lowThreshold)
            {
                usageBar.color = lowColor;
            }
            else
            {
                usageBar.color = normalColor;
            }
        }
    }

    // Metode untuk memulai penggunaan partikel
    public bool StartUsingParticle()
    {
        if (currentUsage > 0)
        {
            isUsingParticle = true;
            return true;
        }
        return false;
    }

    // Metode untuk menghentikan penggunaan partikel
    public void StopUsingParticle()
    {
        isUsingParticle = false;
    }

    // Mulai efek kedip
    private void StartBlinking()
    {
        if (!isBlinking)
        {
            isBlinking = true;
            StartCoroutine(Blink());
        }
    }

    // Hentikan efek kedip
    private void StopBlinking()
    {
        if (isBlinking)
        {
            isBlinking = false;
            StopCoroutine(Blink());
            usageBar.color = normalColor; // Set warna kembali ke normal setelah berhenti berkedip
        }
    }

    // Coroutine untuk efek kedip
    private IEnumerator Blink()
    {
        while (isBlinking)
        {
            // Fade out
            float elapsedTime = 0f;
            while (elapsedTime < blinkDuration)
            {
                usageBar.color = Color.Lerp(lowColor, new Color(lowColor.r, lowColor.g, lowColor.b, 0), elapsedTime / blinkDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Fade in
            elapsedTime = 0f;
            while (elapsedTime < blinkDuration)
            {
                usageBar.color = Color.Lerp(new Color(lowColor.r, lowColor.g, lowColor.b, 0), lowColor, elapsedTime / blinkDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}
