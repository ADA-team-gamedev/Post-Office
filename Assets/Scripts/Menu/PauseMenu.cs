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
	}

	private void Start()
    {
        Time.timeScale = 1.0f;

		_pauseMenu.SetActive(false);
		_exitWindow.SetActive(false);
		_settingsWindow.SetActive(false);
		_playerCrosshair.SetActive(false);
		_playerStaminaBar.SetActive(false);
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
		}

		_pauseMenu.SetActive(_isPaused);

		_playerCrosshair.SetActive(!_isPaused);

		_playerStaminaBar.SetActive(!_isPaused);
	}

	public void OnResumeButton()
	{
		Time.timeScale = 0;

		Cursor.lockState = CursorLockMode.Locked;

		_isPaused = false;

		_pauseMenu.SetActive(false);

		_playerCrosshair.SetActive(true);

		_playerStaminaBar.SetActive(true);
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

	#endregion

	#region Settings

	#endregion

	#region Exit Panel

	#endregion



	public void OnExitToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void OnExitToDesktop()
    {
        Application.Quit();
    }

	private void OnEnable()
	{
        _playerInput.Enable();
	}

	private void OnDisable()
	{
		_playerInput.Disable();
	}
}
