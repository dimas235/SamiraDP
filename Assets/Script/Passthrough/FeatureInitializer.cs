using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class FeatureInitializer : MonoBehaviour
{
    public FadeMaterial fadeMaterial;  // Referensi ke skrip FadeMaterial
    public Toggle passthroughToggle;   // Referensi ke Toggle untuk passthrough
    public ARPlaneManager arPlaneManager; // Referensi ke ARPlaneManager

    private void Start()
    {
        // Aktifkan fitur-fitur ketika permainan dimulai
        ActivateFeatures();
    }

    private void ActivateFeatures()
    {
        // Mengaktifkan fade skybox
        if (fadeMaterial != null)
        {
            fadeMaterial.FadeSkybox(true);
        }

        // Mengaktifkan passthrough
        if (passthroughToggle != null)
        {
            passthroughToggle.isOn = true;
            passthroughToggle.onValueChanged.AddListener(TogglePassthrough);
        }

        // Mengaktifkan AR Plane Manager
        if (arPlaneManager != null)
        {
            StartCoroutine(TurnOnPlanes());
        }
    }

    private IEnumerator TurnOnPlanes()
    {
        yield return new WaitForSeconds(1f);  // Tunggu 1 detik sebelum mengaktifkan AR Plane Manager
        arPlaneManager.enabled = true;
    }

    public void TogglePassthrough(bool isOn)
    {
        // Mengaktifkan atau menonaktifkan passthrough
        if (isOn)
        {
            // Logika untuk mengaktifkan passthrough
            Debug.Log("Passthrough diaktifkan");
        }
        else
        {
            // Logika untuk menonaktifkan passthrough
            Debug.Log("Passthrough dinonaktifkan");
        }
    }
}
