using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject playGameCanvas;
    public GameObject canvasClearTutorial;
    public GameObject[] uiCanvases;
    public GameObject CanvasSetUp;
    public Button playButton;
    public Button tutorialButton;
    public Button NextToMainMenu;
    public Button nextButton;
    public Button mainMenuButton; // Tambahkan referensi untuk tombol Main Menu
    public float clearTutorialDelay = 2f;
    private FireSpawner fireSpawner;

    private void Start()
    {
        fireSpawner = FindObjectOfType<FireSpawner>();
        if (fireSpawner == null)
        {
            Debug.LogError("FireSpawner tidak ditemukan di scene.");
            return;
        }

        playButton.onClick.AddListener(OnPlayButtonClick);
        tutorialButton.onClick.AddListener(OnTutorialButtonClick);
        nextButton.onClick.AddListener(OnNextButtonClick);
        mainMenuButton.onClick.AddListener(OnMainMenuButtonClick); // Tambahkan listener untuk tombol Main Menu
        NextToMainMenu.onClick.AddListener(OnToMainMenu);

        // Pastikan semua objek api dinonaktifkan pada awalnya
        fireSpawner.DeactivateAllFires();
    }

    private void OnPlayButtonClick()
    {
        mainMenuCanvas.SetActive(false);
        playGameCanvas.SetActive(true);
    }

    private void OnTutorialButtonClick()
    {
        playGameCanvas.SetActive(false);

        Vector3 initialPosition = new Vector3(0, 0, 0);
        float spacing = 0.8f;

        for (int i = 0; i < uiCanvases.Length; i++)
        {
            GameObject uiCanvas = uiCanvases[i];
            Vector3 targetPosition = initialPosition + new Vector3(i * spacing, 0, 0);

            uiCanvas.SetActive(true);
            uiCanvas.transform.localPosition = new Vector3(-800f, 0, 0);

            uiCanvas.transform.DOLocalMove(targetPosition, 1f).SetDelay(i * 0.5f);
        }
    }

    public void UseParticle()
    {
        bool anyActive = false;

        foreach (var fire in fireSpawner.currentFireObjects)
        {
            if (fire != null && fire.activeSelf)
            {
                anyActive = true;
                break;
            }
        }

        if (anyActive)
        {
            fireSpawner.DeactivateAllFires();
            Debug.Log("All fires deactivated.");
        }
        else
        {
            fireSpawner.SpawnFiresAtSpawnPoints();
            Debug.Log("Fires spawned.");
        }

        DeactivateAllCanvases();
    }

    public void DeactivateAllCanvases()
    {
        foreach (var canvas in uiCanvases)
        {
            if (canvas.activeSelf)
            {
                canvas.SetActive(false);
            }
        }
    }

    public void CheckAllFiresExtinguished()
    {
        bool allExtinguished = true;

        foreach (var fire in fireSpawner.currentFireObjects)
        {
            if (fire != null)
            {
                Fire fireComponent = fire.GetComponent<Fire>();
                if (fireComponent != null && !fireComponent.IsExtinguished())
                {
                    allExtinguished = false;
                    break;
                }
            }
        }

        if (allExtinguished)
        {
            StartCoroutine(DestroyRemainingFiresWithDelay());
            StartCoroutine(ShowClearTutorialWithDelay());
        }
    }

    private IEnumerator ShowClearTutorialWithDelay()
    {
        yield return new WaitForSeconds(clearTutorialDelay);
        canvasClearTutorial.SetActive(true);
    }

    private IEnumerator DestroyRemainingFiresWithDelay()
    {
        yield return new WaitForSeconds(clearTutorialDelay);
        fireSpawner.DestroyRemainingFires();
    }

    private void OnNextButtonClick()
    {
        canvasClearTutorial.SetActive(false);

        Vector3 initialPosition = new Vector3(0, 0, 0);
        float spacing = 0.8f;

        for (int i = 0; i < uiCanvases.Length; i++)
        {
            GameObject uiCanvas = uiCanvases[i];
            Vector3 targetPosition = initialPosition + new Vector3(i * spacing, 0, 0);

            uiCanvas.SetActive(true);
            uiCanvas.transform.localPosition = new Vector3(-800f, 0, 0);

            uiCanvas.transform.DOLocalMove(targetPosition, 1f).SetDelay(i * 0.5f);
        }
    }

    public void OnMainMenuButtonClick()
    {
        // Menonaktifkan semua canvas
        DeactivateAllCanvases();

        // Mengaktifkan canvas main menu
        mainMenuCanvas.SetActive(true);

        // Mengatur ulang game ke kondisi awal
        playGameCanvas.SetActive(false);
        canvasClearTutorial.SetActive(false);
        fireSpawner.DeactivateAllFires();
    }

    public void OnToMainMenu()
    {
        DeactivateAllCanvases();
        mainMenuCanvas.SetActive(true);
        fireSpawner.DeactivateAllFires();
        CanvasSetUp.SetActive(false);
        fireSpawner.RespawnFires(); // Panggil RespawnFires untuk menghancurkan api dan reset flag
        fireSpawner.EnableSpawning(); // Panggil EnableSpawning untuk menambahkan titik spawn
    }
}
