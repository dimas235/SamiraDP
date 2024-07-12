using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public GameObject fireInformationCanvas;
    public GameObject mainMenuCanvas;
    public GameObject playGameCanvas;
    public GameObject canvasClearTutorial;
    public GameObject TutorialCanvas;
    public GameObject[] uiCanvases;
    public GameObject CanvasSetUp;
    public Button playButton;
    public Button tutorialButton;
    public Button NextToMainMenu;
    public Button nextButton;
    public Button mainMenuButton; // Tambahkan referensi untuk tombol Main Menu
    public Button quitButton; // Tambahkan referensi untuk tombol Quit
    public Button NextToInformation;
    public Button goToTutorialSetUp;
    public Button goToFireInformation;
    public Button backFireInformation;
    public float clearTutorialDelay = 2f;
    private FireSpawner fireSpawner;
    private int currentStage = 0; // Stage saat ini

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
        quitButton.onClick.AddListener(OnQuitButtonClick); // Tambahkan listener untuk tombol Quit
        NextToMainMenu.onClick.AddListener(OnToMainMenu);
        NextToInformation.onClick.AddListener(OnToInformation);
        goToTutorialSetUp.onClick.AddListener(OnToTutorial);
        goToFireInformation.onClick.AddListener(OnToFireIndormation);
        backFireInformation.onClick.AddListener(OnBackFireInformation);

        // Pastikan semua objek api dinonaktifkan pada awalnya
        fireSpawner.DeactivateAllFires();

        // Panggil metode untuk memperbarui status tombol saat memulai
        FindObjectOfType<ButtonApar>().UpdateButtonStates();
    }

    private void OnToFireIndormation()
    {
        DeactivateAllCanvases();
        mainMenuCanvas.SetActive(false);
        playGameCanvas.SetActive(false);
        fireInformationCanvas.SetActive(true);
    }

    private void OnBackFireInformation()
    {
        DeactivateAllCanvases();
        fireInformationCanvas.SetActive(false);
        mainMenuCanvas.SetActive(false);
        playGameCanvas.SetActive(true);

        AudioManager.instance.StopSFX();
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
        currentStage++;
        FindObjectOfType<ButtonApar>().UpdateButtonStates();
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
        currentStage = 0; // Reset stage ke awal
        FindObjectOfType<ButtonApar>().UpdateButtonStates(); // Perbarui status tombol
    }

    public void OnToMainMenu()
    {
        Debug.Log("Memulai transisi ke Main Menu");

        // Menonaktifkan semua kanvas
        DeactivateAllCanvases();
        Debug.Log("Semua kanvas dinonaktifkan");

        // Mengaktifkan kanvas Main Menu
        mainMenuCanvas.SetActive(true);
        Debug.Log("Kanvas Main Menu diaktifkan");

        // Menonaktifkan semua api
        fireSpawner.DeactivateAllFires();
        Debug.Log("Semua api dinonaktifkan");

        // Menonaktifkan kanvas SetUp
        CanvasSetUp.SetActive(false);
        Debug.Log("Kanvas SetUp dinonaktifkan");

        // Menghancurkan api dan mereset flag
        fireSpawner.RespawnFires();
        Debug.Log("Api dihancurkan dan flag direset");

        // Menambahkan titik spawn
        fireSpawner.EnableSpawning();
        Debug.Log("Titik spawn ditambahkan");

        // Mereset stage ke awal
        currentStage = 0;
        Debug.Log("Stage direset ke awal");

        // Memperbarui status tombol
        ButtonApar buttonApar = FindObjectOfType<ButtonApar>();
        if (buttonApar != null)
        {
            buttonApar.UpdateButtonStates();
            Debug.Log("Status tombol diperbarui");
        }
        else
        {
            Debug.LogWarning("ButtonApar tidak ditemukan");
        }

        Debug.Log("Transisi ke Main Menu selesai");
    }

    private void OnQuitButtonClick()
    {
        Debug.Log("Keluar dari aplikasi.");
        Application.Quit();
    }

    public void OnToInformation()
    {
        DeactivateAllCanvases();
        CanvasSetUp.SetActive(true);
        fireSpawner.DeactivateAllFires();
        TutorialCanvas.SetActive(false);
    }

    public void OnToTutorial()
    {
        DeactivateAllCanvases();
        TutorialCanvas.SetActive(true);
        CanvasSetUp.SetActive(false);
    }

    public int GetCurrentStage()
    {
        return currentStage;
    }
}
