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

        uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("UIManager tidak ditemukan di scene.");
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

        // Set initial button states setelah uiManager diinisialisasi
        UpdateButtonStates();
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

    public void UpdateButtonStates()
    {
        if (uiManager == null)
        {
            return;
        }

        int stage = uiManager.GetCurrentStage();

        for (int i = 0; i < buttons.Count; i++)
        {
            if (i <= stage)
            {
                buttons[i].interactable = true;
                SetButtonTransparency(buttons[i], 1f);
            }
            else
            {
                buttons[i].interactable = false;
                SetButtonTransparency(buttons[i], 0.5f);
            }
        }
    }

    private void SetButtonTransparency(Button button, float alpha)
    {
        ColorBlock colors = button.colors;
        Color disabledColor = colors.disabledColor;
        disabledColor.a = alpha;
        colors.disabledColor = disabledColor;
        button.colors = colors;
    }
}
