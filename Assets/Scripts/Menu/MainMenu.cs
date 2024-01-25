using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	[Header("Menus")]
	[SerializeField] private GameObject _settingsWindow;

	private void Awake()
	{
		_settingsWindow.SetActive(false);
	}

	public void OnResumeButton()
	{
		//SceneManager.LoadScene("Game"); //write here game scene
	}

	public void OnNewGameButton()
	{
		//reset level data and create new, then load game scene
	}

	public void OnLoadButton()
	{
		//Load game data with level
	}

	#region Settings

	#endregion

	public void OnExit()
	{
		Application.Quit();
	}
}
