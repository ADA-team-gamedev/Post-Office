using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum MenuButtonsType
{
    NewGame,
    LoadGame,
    Settings,
    Quit,
    CloseSettings,
}
public class MainButtons : MonoBehaviour
{
    [SerializeField] private GameObject quitConfirmWindow, settingsWindow;
    [SerializeField] private GameObject[] settingsSections;
    [SerializeField] private MenuButtonsType type;

    public void Button()
    {
        switch(type) 
        {
            case MenuButtonsType.NewGame:
                SceneManager.LoadScene(1);
                break;
            case MenuButtonsType.LoadGame:
                return;
            case MenuButtonsType.Settings:
                settingsWindow.SetActive(true);
                break;
            case MenuButtonsType.CloseSettings:
                settingsWindow.SetActive(false);
                break;
            case MenuButtonsType.Quit:
                quitConfirmWindow.SetActive(true);
                break;
            default: return;
        }
    }
    public void CloseAllSettingsWindows()
    {
        foreach (GameObject window in settingsSections) 
            window.SetActive(false);
    }
    public void OpenSettingsWindow(GameObject window)
    {
        window.SetActive(true);
    }

    public void Confirmation(int type)
    {
        if (type == 0)
            Application.Quit();
        else
            quitConfirmWindow.SetActive(false);
    }
}
