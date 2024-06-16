using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireIndicator : MonoBehaviour
{
    public Fire fire; // Reference to the Fire script
    public Image hpImage; // Reference to the HP Circle Image
    public Transform vfxParent; // Parent Transform containing VFX children
    public List<Transform> excludedVFX; // List of VFX Transforms to exclude from scaling
    public GameObject healthBar; // Reference to the Health Bar GameObject
    private float currentHp; // Current displayed HP
    private List<Transform> fireVFXList; // List of VFX Transforms to scale
    private List<Vector3> initialScales; // List to store initial scales

    // Start is called before the first frame update
    void Start()
    {
        if (fire == null)
        {
            fire = GetComponentInChildren<Fire>(); // Get Fire component from child objects
        }

        if (fire != null)
        {
            currentHp = fire.hp;
        }

        fireVFXList = new List<Transform>();
        initialScales = new List<Vector3>();

        if (vfxParent != null)
        {
            foreach (Transform vfxTransform in vfxParent)
            {
                if (!excludedVFX.Contains(vfxTransform))
                {
                    fireVFXList.Add(vfxTransform);
                    initialScales.Add(vfxTransform.localScale);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (fire != null && hpImage != null)
        {
            // Smoothly interpolate currentHp towards fire.hp
            currentHp = Mathf.Lerp(currentHp, fire.hp, Time.deltaTime * 3f); // The 5f here is the speed of interpolation, you can adjust it

            // Update the fill amount of the hpImage
            hpImage.fillAmount = currentHp / 100f; // Assuming max HP is 100

            // Update the scale of each VFX based on current HP
            for (int i = 0; i < fireVFXList.Count; i++)
            {
                if (fireVFXList[i] != null)
                {
                    float scaleMultiplier = currentHp / 100f; // Assuming max HP is 100
                    fireVFXList[i].localScale = initialScales[i] * scaleMultiplier;
                }
            }

            // Check if the fire's HP has reached 0 and start the shrinking process for excluded VFX
            if (fire.hp <= 0 && !IsInvoking(nameof(StartShrinkingExcludedVFX)))
            {
                Invoke(nameof(StartShrinkingExcludedVFX), 0f);
            }
        }
    }

    private void StartShrinkingExcludedVFX()
    {
        if (healthBar != null)
        {
            healthBar.SetActive(false); // Deactivate the health bar
        }

        foreach (Transform vfxTransform in excludedVFX)
        {
            if (vfxTransform != null)
            {
                StartCoroutine(ShrinkVFX(vfxTransform, 2f));
            }
        }
    }

    private IEnumerator ShrinkVFX(Transform vfxTransform, float duration)
    {
        Vector3 initialScale = vfxTransform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float scaleMultiplier = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            vfxTransform.localScale = initialScale * scaleMultiplier;
            yield return null;
        }

        vfxTransform.localScale = Vector3.zero;
    }
}
