using Audio;
using Player;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace TaskSystem.NoteBook
{
	public class Tablet : MonoBehaviour
	{
		#region Text

		[Header("Text UI")]
		[SerializeField] private TextMeshProUGUI _taskName;
		[SerializeField] private TextMeshProUGUI _taskDescription;

		[Header("Hint Text")]
		[SerializeField] private TextMeshProUGUI _hintText;
		[SerializeField] private float _hintTextDisplaingDelay = 5f;

		[SerializeField] private GameObject _taskHintArrows;

		[Header("MiniMap")]
		[SerializeField] private Camera _miniMapCamera;
		[SerializeField, Min(0.01f)] private float _zoomSpeed = 0.1f;
		[SerializeField, Min(1)] private float _maxCameraSizeInPercent = 2f;
		[SerializeField, Min(0.1f)] private float _minCameraSizeInPercent = 1f;

		private float _minCameraMapSize;
		private float _deffaultMapCameraSize;
		private float _maxCameraMapSize;

		private float _zoomTime = 0f;

		private string _emptyTextField => string.Empty;

		private const string _addedTaskHint = "Added new tak";
		private const string _selectedNewTaskHint = "Selected new task";
		private const string _completedTaskHint = "Task is completed";

		#endregion

		[Header("Tablet Screen Info")]
		[SerializeField] private Transform _tabletScreenInfo;

		[SerializeField, Range(10f, 50f)] private float _screenInfoAnimationSpeed = 40f;
		private float _deffaultScreenInfoScale;
		private const float _minScreenInfoScale = 0;

		[Header("Values")]
		[SerializeField] private float _animationSpeed = 2f;

		[SerializeField] private Vector3 _openedPositionOffset = new(0.18f, 0.22f, 0);

		[Header("Player Death")]
		[SerializeField] private PlayerDeathController _playerDeathController;

		public bool IsViewing { get; private set; } = false;

		private Vector3 _defaultPosition;	

		private int _taskIndex = -1;

		private PlayerInput _playerInput;

		[Inject]
		private void Construct(PlayerInput playerInput)
		{
			_playerInput = playerInput;

			_playerInput.Player.NoteBook.performed += OnNoteBook;
			_playerInput.Player.NoteBook.canceled += OnNoteBook;

			_playerInput.Player.ScrollWheelY.performed += OnTaskScroll;

			_playerInput.Player.ZoomMapIn.performed += OnZoomValueChanged;

			_playerInput.Player.ZoomMapOut.performed += OnZoomValueChanged;
		}

		private void Awake()
		{
			_defaultPosition = transform.position;

			_playerDeathController.OnDied += DisableNoteBook;

			_deffaultScreenInfoScale = _tabletScreenInfo.transform.localScale.x;

			_deffaultMapCameraSize = _miniMapCamera.orthographicSize;

			_minCameraMapSize = _miniMapCamera.orthographicSize * _minCameraSizeInPercent;

			_maxCameraMapSize = _miniMapCamera.orthographicSize * _maxCameraSizeInPercent;
		}

		private void Update()
		{
			if (IsViewing)
				OpenNoteBook();
			else
				CloseNoteBook();

			if (_playerInput.Player.ZoomMapIn.IsPressed())
				ZoomIn();
			else if (_playerInput.Player.ZoomMapOut.IsPressed())
				ZoomOut();
			else
				ZoomToDeffault();
		}

		public void SubcribeOnTaskManager()
		{
			ClearNotebookTaskInfo();

			StartCoroutine(ClearHintTextField(0));

			ChangeArrowState();

			TaskManager.Instance.OnAddedNewTask += OnAddedTaskHint;

			TaskManager.Instance.OnAddedNewTask += ChangeArrowState;

			TaskManager.Instance.OnNewCurrentTaskSet += WriteTextInNoteBook;

			TaskManager.Instance.OnNewCurrentTaskSet += OnNewCurrentTaskSetHint;

			TaskManager.Instance.OnCurrentTaskCompleted += ClearNotebookTaskInfo;

			TaskManager.Instance.OnCurrentTaskCompleted -= OnCurrentTaskCompletedHint;

			TaskManager.Instance.OnTaskCompleted += ChangeArrowState;

			_taskIndex = TaskManager.Instance.TaskCount - 1;
		}

		#region Input

		private void OnNoteBook(InputAction.CallbackContext context)
		{
			if (context.performed)
				IsViewing = !IsViewing;					
			else if (context.canceled)
				IsViewing = false;

			AudioManager.Instance.PlaySound("On Tablet", transform.position);
		}

		private void OnTaskScroll(InputAction.CallbackContext context)
		{
			if (!IsViewing)
				return;
			
			float scrollWheelValue = context.ReadValue<float>();

			if (scrollWheelValue != 0)
			{
				if (scrollWheelValue > 0)
					_taskIndex++;
				else
					_taskIndex--;

				if (_taskIndex < 0)
					_taskIndex = TaskManager.Instance.TaskCount - 1;
				else if (_taskIndex > TaskManager.Instance.TaskCount - 1)
					_taskIndex = 0;

				TaskManager.Instance.SetNewCurrentTask(_taskIndex);
			}
		}

		private void OnZoomValueChanged(InputAction.CallbackContext context)
		{
			_zoomTime = 0f;
		}

		#region MiniMap

		private void ZoomIn()
		{
			if (!IsViewing || !_playerInput.Player.ZoomMapIn.IsPressed())
				return;

			_zoomTime += _zoomSpeed * Time.deltaTime;

			_miniMapCamera.orthographicSize = Mathf.Lerp(_miniMapCamera.orthographicSize, _minCameraMapSize, _zoomTime);
		}

		private void ZoomOut()
		{
			if (!IsViewing || !_playerInput.Player.ZoomMapOut.IsPressed())
				return;

			_zoomTime += _zoomSpeed * Time.deltaTime;

			_miniMapCamera.orthographicSize = Mathf.Lerp(_miniMapCamera.orthographicSize, _maxCameraMapSize, _zoomTime);
		}

		private void ZoomToDeffault()
		{
			if (!IsViewing)
				return;

			_zoomTime += _zoomSpeed * Time.deltaTime;

			_miniMapCamera.orthographicSize = Mathf.Lerp(_miniMapCamera.orthographicSize, _deffaultMapCameraSize, _zoomTime);
		}

		#endregion

		#endregion

		#region NoteBook Animations

		private void OpenNoteBook()
		{
			Vector3 newPosition = _defaultPosition + _openedPositionOffset;

			if (!IsViewing || transform.position == newPosition)
				return;

			transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * _animationSpeed);

			float newScreenInfoXScale = Mathf.Lerp(_tabletScreenInfo.localScale.x, _deffaultScreenInfoScale, Time.deltaTime * _screenInfoAnimationSpeed);

			Vector3 newScreenInfoScale = new(newScreenInfoXScale, _tabletScreenInfo.localScale.y, _tabletScreenInfo.localScale.z);

			_tabletScreenInfo.localScale = newScreenInfoScale;
		}

		private void CloseNoteBook()
		{
			if (IsViewing || transform.position == _defaultPosition)
				return;

			transform.position = Vector3.Lerp(transform.position, _defaultPosition, Time.deltaTime * _animationSpeed);

			float newScreenInfoXScale = Mathf.Lerp(_tabletScreenInfo.localScale.x, _minScreenInfoScale, Time.deltaTime * _screenInfoAnimationSpeed);

			Vector3 newScreenInfoScale = new (newScreenInfoXScale, _tabletScreenInfo.localScale.y, _tabletScreenInfo.localScale.z);

			_tabletScreenInfo.localScale = newScreenInfoScale;
		}

		#endregion

		#region Text Methods

		#region Hint

		private void OnAddedTaskHint()
		{
			WriteHintText(_addedTaskHint, Color.green);
		}

		private void OnNewCurrentTaskSetHint(Task taks)
		{
			WriteHintText(_selectedNewTaskHint, Color.green);
		}

		private void OnCurrentTaskCompletedHint()
		{
			WriteHintText(_completedTaskHint, Color.green);
		}

		private void ChangeArrowState()
		{
			_taskHintArrows.SetActive(TaskManager.Instance.TaskCount > 1);
		}

		public void WriteHintText(string hintText, Color textColor)
		{
			StopAllCoroutines();
			
			_hintText.color = textColor;

			_hintText.text = hintText;
			
			StartCoroutine(ClearHintTextField(_hintTextDisplaingDelay));
		}

		private IEnumerator ClearHintTextField(float delay)
		{
			yield return new WaitForSeconds(delay);

			_hintText.text = _emptyTextField;
		}

		#endregion

		private void WriteTextInNoteBook(Task task)
		{
			//FontStyles fontStyle = _defaultFontStyle;

			//if (task.IsCompleted)
			//	fontStyle += (int)FontStyles.Strikethrough;

			//_taskName.fontStyle = fontStyle;

			//_taskDescription.fontStyle = fontStyle;

			_taskName.text = task.Name;

			_taskDescription.text = task.Description;
		}

		private void ClearNotebookTaskInfo()
		{
			_taskName.text = "";

			_taskDescription.text = "";

			if (_taskIndex == 0)
				_taskIndex = TaskManager.Instance.TaskCount - 1;
			else if (_taskIndex > 0)
				_taskIndex--;
		}

		#endregion

		private void DisableNoteBook()
		{
			_playerDeathController.OnDied -= DisableNoteBook;

			_playerInput.Player.NoteBook.performed -= OnNoteBook;
			_playerInput.Player.NoteBook.canceled -= OnNoteBook;

			_playerInput.Player.ScrollWheelY.performed -= OnTaskScroll;

			Destroy(this);
		}

		private void OnValidate()
		{
			if (_minCameraSizeInPercent > 1f)
				_minCameraSizeInPercent = 1f;
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;

			Vector3 tabletPosition = Application.isPlaying ? _defaultPosition : transform.position;

			Gizmos.DrawWireSphere(tabletPosition + _openedPositionOffset, 0.01f);
		}

		private void OnDestroy()
		{
			if (_playerInput != null)
			{
				_playerInput.Player.NoteBook.performed -= OnNoteBook;
				_playerInput.Player.NoteBook.canceled -= OnNoteBook;

				_playerInput.Player.ScrollWheelY.performed -= OnTaskScroll;

				_playerInput.Player.ZoomMapIn.performed -= OnZoomValueChanged;

				_playerInput.Player.ZoomMapOut.performed -= OnZoomValueChanged;
			}

			_playerDeathController.OnDied -= DisableNoteBook;

			TaskManager.Instance.OnAddedNewTask -= OnAddedTaskHint;

			TaskManager.Instance.OnAddedNewTask -= ChangeArrowState;

			TaskManager.Instance.OnNewCurrentTaskSet -= WriteTextInNoteBook;

			TaskManager.Instance.OnNewCurrentTaskSet -= OnNewCurrentTaskSetHint;

			TaskManager.Instance.OnCurrentTaskCompleted -= ClearNotebookTaskInfo;

			TaskManager.Instance.OnCurrentTaskCompleted -= OnCurrentTaskCompletedHint;

			TaskManager.Instance.OnTaskCompleted -= ChangeArrowState;
		}
	}
}