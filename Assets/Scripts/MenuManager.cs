using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] GameObject pauseMenu;

    [SerializeField] string mainMenuName = "MenuMain";

    public bool isPaused;

    // Start is called before the first frame update
    void Awake()
    {
        // Prevent multiple gameManagers from existing (keep existing game data)
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // Data persistance
        Instance = this;
        DontDestroyOnLoad(this);

        MenuManager.Instance.FindPauseMenu();
    }

    public void FindPauseMenu()
    {
        // Find pause menu
        pauseMenu = GameObject.Find("UIManager/PauseMenu");
        pauseMenu.SetActive(false);
        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartFirstGameScene()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
    }

    public void QuitGame()
    {
        Debug.Log("Quit the Game");
        Application.Quit();
    }
    public void PauseGame()
    {
        Debug.Log("Pause!");
        GameManager.Instance.GetCurrentPlayer().GetComponent<PlayerType>().OnGameControlDisable();

        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        isPaused = true;
        
    }

    // Specific to pause menu
    public void ResumeGame()
    {
        GameManager.Instance.GetCurrentPlayer().GetComponent<PlayerType>().OnGameControlEnable();
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        isPaused = false;
    }

    public void EnterMainMenu()
    {
        SceneManager.LoadScene(mainMenuName);
        Time.timeScale = 1;
    }
}
