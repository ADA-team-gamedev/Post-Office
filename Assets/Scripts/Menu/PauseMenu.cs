using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
	[Header("Menus")]
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _exitWindow;
    [SerializeField] private GameObject _settingsWindow;

	[Header("Player")]
	[SerializeField] private GameObject _playerCrosshair;
	[SerializeField] private GameObject _playerStaminaBar;

	private PlayerInput _playerInput;

    private bool _isPaused = false;

	private void Awake()
	{
        _playerInput = new();

        _playerInput.UI.PauseMenu.performed += OnPauseMenu;

		_exitWindow.SetActive(false);

		_settingsWindow.SetActive(false);
	}

	private void Start()
    {
		OnResumeButton();
	}

	#region Pause Menu

	private void OnPauseMenu(InputAction.CallbackContext context)
	{
		_isPaused = !_isPaused;

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
	}

	public void OnResumeButton()
	{
		Time.timeScale = 1f;

		Cursor.lockState = CursorLockMode.Locked;

		_isPaused = false;

		_exitWindow.SetActive(false);

		_settingsWindow.SetActive(false);

		_pauseMenu.SetActive(false);

		_playerCrosshair.SetActive(true);

		_playerStaminaBar.SetActive(true);	
	}

	public void OnNewGameButton()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void OnSaveButton()
	{
		return;
	}

	public void OnLoadButton()
	{
		return;
	}

	#endregion

	#region Settings

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

	private void OnEnable()
	{
        _playerInput.Enable();
	}

	private void OnDisable()
	{
		_playerInput.Disable();
	}
}
