using DataPersistance;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private Slider _loadingProgressBar;
    [SerializeField] private TMP_Text _canLoadSceneHint;

    private AsyncOperation _asyncOperation;

	private PlayerInput _playerInput;

	private IDataService _dataService = new JsonDataService();

	public const string LoadingSceneName = "Loading";
	public const string LoadingInfoPath = "/LoadingInfo";

	private void Start()
	{
		_playerInput.Player.AnyKey.performed += OnAnyKeyPressed;

		_canLoadSceneHint.gameObject.SetActive(false);

		_loadingProgressBar.value = 0;
		
		if (_dataService.TryLoadData(out string loadedSceneName, LoadingInfoPath, true))
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

	private void OnEnable()
	{
		_playerInput = new();

		_playerInput.Enable();
	}

	private void OnDisable()
	{
		_playerInput.Disable();
	}
}
