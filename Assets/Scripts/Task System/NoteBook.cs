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

	private bool _isViewing = false;

    private Vector3 _defaultPosition;

    private PlayerInput _playerInput;

	private void Awake()
	{
		_playerInput = new();

		_playerInput.UI.NoteBook.performed += OnNoteBook;
		_playerInput.UI.NoteBook.canceled += OnNoteBook;

		_playerInput.Player.ScrollWheelY.performed += OnTaskScroll;

		ClearNotebook();

		TaskManager.Instance.OnNewCurrentTaskSet += WriteTextInNoteBook;

		TaskManager.Instance.CurrentTaskCompleted += ClearNotebook;
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

		int taskIndex = -1;

		float scrollWheelValue = context.ReadValue<float>();

		if (scrollWheelValue != 0)
		{
			if (scrollWheelValue > 0)
				taskIndex++;
			else
				taskIndex--;

			if (taskIndex < 0)
				taskIndex = TaskManager.Instance.TaskCount - 1;
			else if (taskIndex > TaskManager.Instance.TaskCount - 1)
				taskIndex = 0;

			TaskManager.Instance.SetNewCurrentTask(taskIndex);
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

		_taskName.text = task.Name;

		_taskDescription.text = task.Description;
	}

	private void ClearNotebook()
	{
		_taskName.text = "";

		_taskDescription.text = "";
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
