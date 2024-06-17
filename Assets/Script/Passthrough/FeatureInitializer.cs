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
}
