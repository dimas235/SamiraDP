using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonApar : MonoBehaviour
{
    public List<GameObject> particles; // Daftar partikel yang akan diaktifkan dan dinonaktifkan
    public List<Button> buttons; // Daftar tombol untuk mengaktifkan partikel
    private IconManager iconManager;
    private UIManager uiManager;

    private void Start()
    {
        iconManager = FindObjectOfType<IconManager>();
        if (iconManager == null)
        {
            Debug.LogError("IconManager tidak ditemukan di scene.");
            return;
        }

        // Tambahkan listener ke setiap tombol
        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i; // Menyimpan indeks untuk digunakan dalam lambda
            buttons[i].onClick.AddListener(() => ActivateParticle(index));
        }

        // Pastikan tidak ada partikel yang aktif pada awalnya
        DeactivateAllParticles();
        iconManager.SetDefaultIcon();
    }

    private void ActivateParticle(int index)
    {
        // Nonaktifkan semua partikel terlebih dahulu
        DeactivateAllParticles();

        // Aktifkan partikel yang sesuai dengan indeks tombol yang diklik
        if (index >= 0 && index < particles.Count)
        {
            particles[index].SetActive(true);
            Debug.Log($"Partikel {particles[index].name} diaktifkan.");
            iconManager.UpdateIconByParticle(particles[index]);
        }
        else
        {
            Debug.LogError("Indeks partikel di luar batas.");
        }
    }

    private void DeactivateAllParticles()
    {
        foreach (var particle in particles)
        {
            particle.SetActive(false);
        }
    }

    public void OnUseButtonClick()
    {
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("UIManager tidak ditemukan di scene.");
            return;
        }

        uiManager.UseParticle();
    }
}
