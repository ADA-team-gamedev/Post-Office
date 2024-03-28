using Audio;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TaskSystem.NoteBook
{
	public class NoteBook : MonoBehaviour
	{
		[Header("Text UI")]
		[SerializeField] private TextMeshProUGUI _taskName;
		[SerializeField] private TextMeshProUGUI _taskDescription;

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

		private PlayerInput _playerInput;

		private FontStyles _defaultFontStyle;

		private int _taskIndex = -1;

		private void Awake()
		{
			_defaultPosition = transform.position;

			_playerDeathController.OnDeath += DisableNoteBook;

			_playerInput = new();

			_playerInput.UI.NoteBook.performed += OnNoteBook;
			_playerInput.UI.NoteBook.canceled += OnNoteBook;

			_playerInput.Player.ScrollWheelY.performed += OnTaskScroll;

			ClearNotebook();

			_defaultFontStyle = _taskName.fontStyle;

			_deffaultScreenInfoScale = _tabletScreenInfo.transform.localScale.x;
		}

		private void Start()
		{
			TaskManager.Instance.OnNewCurrentTaskSet += WriteTextInNoteBook;

			TaskManager.Instance.CurrentTaskCompleted += ClearNotebook;

			_taskIndex = TaskManager.Instance.TaskCount - 1;

			if (TaskManager.Instance.CurrentTask != null) //For cases when we add a task at start but we still haven't subscribed to the TaskManager
				WriteTextInNoteBook(TaskManager.Instance.CurrentTask);
		}

		private void Update()
		{
			if (IsViewing)
				OpenNoteBook();
			else
				CloseNoteBook();
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

		#endregion

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

		#region Text Methods

		public void RewriteText(string text)
		{
			_taskDescription.text = text;
		}

		public void AddExtraText(string extraText)
		{
			string currentText = _taskDescription.text;

			_taskDescription.text = $"{currentText}\n{extraText}";
		}

		private void WriteTextInNoteBook(Task task)
		{
			//Play writing text sound

			FontStyles fontStyle = _defaultFontStyle;

			if (task.IsCompleted)
				fontStyle += (int)FontStyles.Strikethrough;

			_taskName.fontStyle = fontStyle;

			_taskDescription.fontStyle = fontStyle;

			_taskName.text = task.Name;

			_taskDescription.text = task.Description;
		}

		private void ClearNotebook()
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
			Destroy(this);
		}

		private void OnEnable()
		{
			_playerInput.Enable();
		}

		private void OnDisable()
		{
			_playerInput.Disable();
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;

			Gizmos.DrawWireSphere(_defaultPosition + _openedPositionOffset, 0.01f);
		}
	}
}