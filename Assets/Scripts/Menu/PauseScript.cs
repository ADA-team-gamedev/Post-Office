using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private KeyCode _pauseKey;
    public GameObject PauseMenu, ExitWindow, SettingsWindow;
    
    private void Start()
    {
        Time.timeScale = 1.0f;
    }
    void Update()
    {
        if (Input.GetKeyDown(_pauseKey))
        {
            if (Time.timeScale != 0)
            {
                Time.timeScale = 0;
                PauseMenu.SetActive(true);
                _playerMovement.CanCameraMove = false;
                _playerMovement.CanPlayerMove = false;
                Cursor.lockState = CursorLockMode.None;
            }
            else
                ExitPause();
        }
    }

    public void OnResumeButton()
    {
        ExitPause();
    }
    public void ExitPause()
    {
        Time.timeScale = 1.0f;
        PauseMenu.SetActive(false);
        _playerMovement.CanCameraMove = true;
        _playerMovement.CanPlayerMove = true;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnNewGameButton()
    {
        SceneManager.LoadScene(1);
    }

    public void OnSaveButton()
    {
        return;
    }

    public void OnLoadButton()
    {
        return;
    }

    public void OnSettingsButton()
    {
        SettingsWindow.SetActive(true);
    }
    public void OnBackButton()
    {
        SettingsWindow.SetActive(false);
    }

    public void OnExitButton()
    {
        ExitWindow.SetActive(true);
    }
    public void OnReturnButton()
    {
        ExitWindow.SetActive(false);
    }
    public void OnExitToMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void OnExitToDesktop()
    {
        Application.Quit();
    }
}
