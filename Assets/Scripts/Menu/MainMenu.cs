using DataPersistance;
using Level;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
	public class MainMenu : MonoBehaviour
	{
		[Header("Menus")]
		[SerializeField] private GameObject _settingsWindow;

		private IDataService _dataService = new JsonDataService();

		private void Start()
		{
			_settingsWindow.SetActive(false);

			Cursor.lockState = CursorLockMode.None;
		}

		public void OnResumeButton(string sceneToLoad)
		{
			if (_dataService.SaveData(SceneLoader.LoadingInfoPath, sceneToLoad, true))
				SceneManager.LoadScene(SceneLoader.LoadingSceneName);
		}

		public void OnNewGameButton(string sceneToLoad)
		{
			WeekDay weekDay = WeekDay.Monday;

			if (_dataService.SaveData(DayObjectLoader.WeekDayPath, weekDay, true))
				OnResumeButton(sceneToLoad);		
		}

		public void OnExit()
		{
			Application.Quit();
		}
	}
}
