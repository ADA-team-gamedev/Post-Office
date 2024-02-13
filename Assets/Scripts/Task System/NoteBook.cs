using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class NoteBook : MonoBehaviour
{
    [Header("Text UI")]
    [SerializeField] private TextMeshProUGUI _taskName;
    [SerializeField] private TextMeshProUGUI _taskDescription;

    [Header("Values")]
	[SerializeField] private float _animationSpeed = 2f;

    [SerializeField] private Vector3 _openedPositionCoefficient = new(0.18f, 0.22f, 0);

	[Header("Player Death")]
	[SerializeField] private PlayerDeathController _playerDeathController;

	private bool _isViewing = false;

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
	}

	private void Start()
	{
		TaskManager.Instance.OnNewCurrentTaskSet += WriteTextInNoteBook;

		TaskManager.Instance.CurrentTaskCompleted += ClearNotebook;

		_taskIndex = TaskManager.Instance.TaskCount - 1;
	}

	private void Update()
	{
		if (_playerInput.UI.NoteBook.IsPressed())
			OpenNoteBook();
		else
			CloseNoteBook();
	}

	#region Input

	private void OnNoteBook(InputAction.CallbackContext context)
	{
		if (context.performed)
			_isViewing = true;
		else if (context.canceled)
			_isViewing = false;
	}

	private void OnTaskScroll(InputAction.CallbackContext context)
	{
		if (!_playerInput.UI.NoteBook.IsPressed())
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
		Vector3 newPosition = _defaultPosition + _openedPositionCoefficient;

		if (!_isViewing || transform.position == newPosition)
			return;

		transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * _animationSpeed);
    }

	private void CloseNoteBook()
    {
		if (_isViewing || transform.position == _defaultPosition)
			return;

		transform.position = Vector3.Lerp(transform.position, _defaultPosition, Time.deltaTime * _animationSpeed);
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

	private void OnValidate()
	{
		_defaultPosition = transform.position;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;

		Gizmos.DrawWireSphere(_defaultPosition + _openedPositionCoefficient, 0.01f);
	}
}
