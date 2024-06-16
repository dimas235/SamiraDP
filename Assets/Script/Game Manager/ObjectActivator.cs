using UnityEngine;

public class ObjectActivator : MonoBehaviour
{
    [Header("Objects to Set Active")]
    public GameObject[] objectsToActivate;

    [Header("Objects to Set Inactive")]
    public GameObject[] objectsToDeactivate;

    private void Start()
    {
        // Set objects to active
        foreach (GameObject obj in objectsToActivate)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }

        // Set objects to inactive
        foreach (GameObject obj in objectsToDeactivate)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }
}
