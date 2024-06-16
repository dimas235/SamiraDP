using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconManager : MonoBehaviour
{
    public Image iconImage; // Referensi ke UI Image untuk ikon
    public Image backgroundImage; // Referensi ke UI Image untuk background warna
    public List<Sprite> icons; // Daftar ikon yang sesuai dengan jenis partikel
    public List<string> particleNames; // Daftar nama skrip partikel
    public List<Color> colors; // Daftar warna yang sesuai dengan jenis partikel
    public Sprite defaultIcon; // Ikon default
    public Color defaultColor; // Warna default

    private void Start()
    {
        SetDefaultIcon(); // Initialize with the default icon and color
    }

    public void UpdateIconByParticle(GameObject particle)
    {
        if (particle != null)
        {
            string particleName = particle.GetComponent<Particle>()?.GetType().Name;
            if (!string.IsNullOrEmpty(particleName))
            {
                int index = particleNames.IndexOf(particleName);
                if (index != -1)
                {
                    iconImage.sprite = icons[index];
                    backgroundImage.color = colors[index];
                    Debug.Log($"Ikon diperbarui ke: {icons[index].name} dan warna background diperbarui ke: {colors[index]} untuk partikel: {particleName}");
                }
                else
                {
                    Debug.LogError($"Nama partikel {particleName} tidak ditemukan dalam daftar particleNames.");
                }
            }
            else
            {
                Debug.LogError("Objek aktif tidak memiliki komponen Particle.");
            }
        }
        else
        {
            SetDefaultIcon();
        }
    }

    public void SetDefaultIcon()
    {
        iconImage.sprite = defaultIcon;
        backgroundImage.color = defaultColor;
        Debug.Log("Ikon diatur ke default.");
    }
}
