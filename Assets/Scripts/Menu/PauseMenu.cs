using DataPersistance;
using Level.Spawners;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Zenject;

namespace Menu
{
	public class PauseMenu : MonoBehaviour
	{
		[Header("Menus")]
		[SerializeField] private GameObject _pauseMenu;
		[SerializeField] private GameObject _exitWindow;
		[SerializeField] private GameObject _settingsWindow;

		[Header("Player")]
		[SerializeField] private GameObject _playerCrosshair;
		[SerializeField] private GameObject _playerStaminaBar;

		private bool _isPaused = false;

		private PlayerInput _playerInput;

		private IDataService _dataService = new JsonDataService();

		private WeekDay _currentWeekDay = WeekDay.Monday;

		[Inject]
		private void Construct(PlayerInput playerInput)
		{
			_playerInput = playerInput;

			_playerInput.UI.PauseMenu.performed += OnPauseMenu;
		}

		private void Awake()
		{
			_exitWindow.SetActive(false);

			_settingsWindow.SetActive(false);

			_dataService.TryLoadData(out _currentWeekDay, JsonDataService.WeekDayPath, true);
		}	

		private void Start()
		{
			OnResumeButton();
		}

		#region Pause Menu

		private void OnPauseMenu(InputAction.CallbackContext context)
		{
			_isPaused = !_isPaused;

			AudioListener.pause = _isPaused;

			if (_isPaused)
			{
				Time.timeScale = 0;

				Cursor.lockState = CursorLockMode.None;
			}
			else
			{
				Time.timeScale = 1;

				Cursor.lockState = CursorLockMode.Locked;

				OnResumeButton();
			}

			_pauseMenu.SetActive(_isPaused);

			_playerCrosshair.SetActive(!_isPaused);

			_playerStaminaBar.SetActive(!_isPaused);
			
			if (_isPaused)
				_playerInput.Player.Disable();	
			else
				_playerInput.Player.Enable();	
		}

		public void OnResumeButton()
		{
			Time.timeScale = 1f;

			Cursor.lockState = CursorLockMode.Locked;

			_isPaused = false;

			AudioListener.pause = false;

			_playerInput.Player.Enable();

			_exitWindow.SetActive(false);

			_settingsWindow.SetActive(false);

			_pauseMenu.SetActive(false);

			_playerCrosshair.SetActive(true);

			_playerStaminaBar.SetActive(true);
		}

		public void OnNewGameButton()
		{
			_currentWeekDay = WeekDay.Monday;

			if (_dataService.SaveData(JsonDataService.WeekDayPath, _currentWeekDay, true))
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		#endregion

		#region Exit Panel

		public void OnExitToMenu()
		{
			SceneManager.LoadScene("Menu");
		}

		public void OnExitToDesktop()
		{
			Application.Quit();
		}

		#endregion
	}
}