using System.Collections;
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

	private void OnNoteBook(InputAction.CallbackContext context)
	{
		if (context.performed)
			_isViewing = true;
		else if (context.canceled)
			_isViewing = false;
	}

	public void OpenNoteBook()
    {
		if (!_isViewing)
			return;

		transform.position = Vector3.Lerp(transform.position, _defaultPosition + _openedPositionCoefficient, Time.deltaTime * _animationSpeed);
    }

    public void CloseNoteBook()
    {
		if (_isViewing)
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
