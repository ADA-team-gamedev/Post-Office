using DataPersistance;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private Slider _loadingProgressBar;
    [SerializeField] private TMP_Text _canLoadSceneHint;

    private AsyncOperation _asyncOperation;

	private IDataService _dataService = new JsonDataService();

	public const string LoadingSceneName = "Loading";

	private PlayerInput _playerInput;

	[Inject]
	private void Construct(PlayerInput playerInput)
	{
		_playerInput = playerInput;

		playerInput.Enable();

		_playerInput.Player.AnyKey.performed += OnAnyKeyPressed;
	}

	private void Start()
	{
		_canLoadSceneHint.gameObject.SetActive(false);

		_loadingProgressBar.value = 0;
		
		if (_dataService.TryLoadData(out string loadedSceneName, JsonDataService.LoadingInfoPath, true))
			StartCoroutine(AsyncSceneLoading(loadedSceneName));		
	}

	private void OnAnyKeyPressed(InputAction.CallbackContext obj)
	{
		if (_asyncOperation != null)
			_asyncOperation.allowSceneActivation = true;
	}

	private IEnumerator AsyncSceneLoading(string nextScene)
    {
		float loadingProgress;

        _asyncOperation = SceneManager.LoadSceneAsync(nextScene);
		
		_loadingProgressBar.gameObject.SetActive(true);

        _asyncOperation.allowSceneActivation = false;

		while (_asyncOperation.progress < 0.9f)
		{
			loadingProgress = Mathf.Clamp01(_asyncOperation.progress / 0.9f);

			_loadingProgressBar.value = loadingProgress;

			yield return null;
		}

		loadingProgress = Mathf.Clamp01(_asyncOperation.progress / 0.9f);

		_loadingProgressBar.value = loadingProgress;

		_canLoadSceneHint.gameObject.SetActive(true);
	}	

	private void OnDestroy()
	{
		if (_playerInput != null)
			_playerInput.Player.AnyKey.performed -= OnAnyKeyPressed;
	}	
}
