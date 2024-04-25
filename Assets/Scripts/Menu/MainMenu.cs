using DataPersistance;
using Level.Spawners;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu
{
	public class MainMenu : MonoBehaviour
	{
		[Header("Menus")]
		[SerializeField] private GameObject _settingsWindow;

		[Header("Buttons")]
		[SerializeField] private Button _resumeButton;

		private readonly IDataService _dataService = new JsonDataService();

		private WeekDay _weekDay;

		private const string _tutorialMapName = "Tutorial";

		private void Start()
		{
			_settingsWindow.SetActive(false);

			Cursor.lockState = CursorLockMode.None;

			if (_dataService.TryLoadData(out _weekDay, JsonDataService.WeekDayPath, true) && _weekDay != WeekDay.Monday)
				_resumeButton.interactable = true;
			else
				_resumeButton.interactable = false;
		}

		public void OnResumeButton(string sceneToLoad)
		{
			if (_dataService.SaveData(JsonDataService.LoadingInfoPath, sceneToLoad, true))
				SceneManager.LoadScene(SceneLoader.LoadingSceneName);
		}

		public void OnNewGameButton(string sceneToLoad)
		{
			WeekDay weekDay = WeekDay.Monday;

			if (_dataService.SaveData(JsonDataService.WeekDayPath, weekDay, true))
				OnResumeButton(sceneToLoad);		
		}

		public void OnTutorialButton()
		{
			SceneManager.LoadScene(_tutorialMapName);
		}

		public void OnExit()
		{
			Application.Quit();
		}
	}
}
