using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraHandler : MonoBehaviour
{
	#region Camera Settings

	[Header("Camera Settigs")]

	[SerializeField] private float _cameraSensitivity = 5f;
	[SerializeField] private float _deffaultFOV = 60f;

	[SerializeField][Range(0, 90)] private float _minCameraLookAngle = 90f;
	[SerializeField][Range(0, 90)] private float _maxCameraLookAngle = 60f;

	[Space(10)]
	[SerializeField] private bool _isCameraInverted = false;

	[Header("Sprinting phase")]
	[SerializeField] private float _sprintFOV = 80f;
	[SerializeField] private float _sprintFOVStepTime = 10f;

	[Header("Objects")]
	[SerializeField] private Camera _playerCamera;
	[SerializeField] private Transform _playerBody;

	// Internal Variables
	private float _yaw = 0f;
	private float _pitch = 0f;

	#endregion

	[SerializeField] private PlayerMovementHandler _playerMovement;

	private PlayerInput _playerInput;

	private PlayerDeathController _playerDeathController;

	private Vector2 _lookDirection;

	private void Awake()
	{
		_playerInput = new();

		_playerInput.Player.Look.performed += OnLook;
		_playerInput.Player.Look.canceled += OnLook;
	}

	private void Start()
	{
		_playerCamera ??= GetComponent<Camera>();	

		Cursor.lockState = CursorLockMode.Locked;

		_playerCamera.fieldOfView = _playerMovement.MovementState == MovementState.Sprinting ? _sprintFOV : _deffaultFOV;
		
		_playerDeathController = transform.parent.parent.GetComponent<PlayerDeathController>(); //Player with DeathController -> HeadJoint -> Camera

		_playerDeathController.OnDeath += DisableCamera;
	}

	private void Update()
	{
		Look();

		ChangeFOV();
	}

	private void Look()
	{
		if (_lookDirection == Vector2.zero)
			return;	

		_yaw = _playerBody.localEulerAngles.y + _lookDirection.x * _cameraSensitivity * Time.deltaTime;

		if (!_isCameraInverted)
			_pitch -= _cameraSensitivity * _lookDirection.y * Time.deltaTime;
		else
			_pitch += _cameraSensitivity * _lookDirection.y * Time.deltaTime;

		_pitch = Mathf.Clamp(_pitch, -_maxCameraLookAngle, _minCameraLookAngle); //max = to floar; min = to sky

		_playerBody.localEulerAngles = new(0, _yaw, 0);
		_playerCamera.transform.localEulerAngles = new(_pitch, 0, 0);
	}

	private void OnLook(InputAction.CallbackContext context)
	{
		_lookDirection = _playerInput.Player.Look.ReadValue<Vector2>();
	}

	private void ChangeFOV()
	{
		if (_playerMovement.MovementState != MovementState.Sprinting || _playerMovement.MoveDirection == Vector2.zero)
		{
			_playerCamera.fieldOfView = Mathf.Lerp(_playerCamera.fieldOfView, _deffaultFOV, _sprintFOVStepTime * Time.deltaTime);

			return;
		}

		_playerCamera.fieldOfView = Mathf.Lerp(_playerCamera.fieldOfView, _sprintFOV, _sprintFOVStepTime * Time.deltaTime);
	}

	private void DisableCamera()
	{
		//Destroy(this);
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
		_playerCamera ??= GetComponent<Camera>();

		_playerCamera.fieldOfView = _deffaultFOV;
	}
}