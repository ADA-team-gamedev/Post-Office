using Audio;
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

		[Header("Pause Menu")]
		[SerializeField] private Animator _pauseMenuAnimator;

		[SerializeField] private GameObject _pauseMenuParent;

		[SerializeField] private string _showPauseMenuTrigger = "Show";
		[SerializeField] private string _hidePauseMenuTrigger = "Hide";

		[SerializeField] private string _pauseMenuSound = "Click";

		[Header("Settings Window")]
		[SerializeField] private Animator _settingsMenuAnimator;

		[SerializeField] private GameObject _settingsMenuParent;

		//[SerializeField] private string _showSettingsMenuTrigger = "Show";
		[SerializeField] private string _hideSettingsMenuTrigger = "Hide";

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

			AudioManager.Instance.PlaySound(_pauseMenuSound, transform.position);

			//AudioListener.pause = _isPaused;

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

			_pauseMenuAnimator.SetTrigger(_isPaused ? _showPauseMenuTrigger : _hidePauseMenuTrigger);

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

			//AudioListener.pause = false;

			_playerInput.Player.Enable();

			if (_settingsMenuParent.activeInHierarchy)
				_settingsMenuAnimator.SetTrigger(_hideSettingsMenuTrigger);

			if (_pauseMenuParent.activeInHierarchy)
				_pauseMenuAnimator.SetTrigger(_hidePauseMenuTrigger);

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
			string sceneToLoad = "Menu";

			if (_dataService.SaveData(JsonDataService.LoadingInfoPath, sceneToLoad, true))
				SceneManager.LoadScene(SceneLoader.LoadingSceneName);
		}

		public void OnExitToDesktop()
		{
			Application.Quit();
		}

		#endregion

		private void OnDestroy()
		{
			if (_playerInput != null)
				_playerInput.UI.PauseMenu.performed -= OnPauseMenu;
		}
	}
}