using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraHandler : MonoBehaviour
{
	#region Camera Settings

	[Header("Camera Settigs")]

	[SerializeField] private float _cameraSensitivity = 5f;
	[SerializeField] private float _deffaultFOV = 60f;
	[SerializeField] private float _maxCameraLookAngle = 60f;

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

	private PlayerInput _playerInput;

	private Vector2 _lookDirection;

	private bool _isSprinting = false;

	private Vector2 _moveDirection;

	private void Awake()
	{
		_playerInput = new();

		_playerInput.Player.Look.performed += OnLook;
		_playerInput.Player.Look.canceled += OnLook;

		_playerInput.Player.Sprint.performed += OnSprint;
		_playerInput.Player.Sprint.canceled += OnSprint;

		_playerInput.Player.Move.performed += OnMove;
		_playerInput.Player.Move.canceled += OnMove;
	}

	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;

		_playerCamera.fieldOfView = _isSprinting ? _sprintFOV : _deffaultFOV;
	}

	private void OnEnable()
	{
		_playerInput.Enable();
	}

	private void OnDisable()
	{
		_playerInput.Disable();
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

		_pitch = Mathf.Clamp(_pitch, -_maxCameraLookAngle, _maxCameraLookAngle);

		_playerBody.localEulerAngles = new(0, _yaw, 0);
		_playerCamera.transform.localEulerAngles = new(_pitch, 0, 0);
	}

	private void OnLook(InputAction.CallbackContext context)
	{
		_lookDirection = _playerInput.Player.Look.ReadValue<Vector2>();
	}

	private void ChangeFOV()
	{
		if (!_isSprinting || _moveDirection == Vector2.zero)
		{
			_playerCamera.fieldOfView = Mathf.Lerp(_playerCamera.fieldOfView, _deffaultFOV, _sprintFOVStepTime * Time.deltaTime);

			return;
		}

		_playerCamera.fieldOfView = Mathf.Lerp(_playerCamera.fieldOfView, _sprintFOV, _sprintFOVStepTime * Time.deltaTime);
	}

	private void OnSprint(InputAction.CallbackContext context)
	{
		if (context.performed)
			_isSprinting = true;		
		else if (context.canceled)
			_isSprinting = false;	
	}

	private void OnMove(InputAction.CallbackContext context)
	{
		_moveDirection = _playerInput.Player.Move.ReadValue<Vector2>();
	}

	private void OnValidate()
	{
		_playerCamera ??= GetComponent<Camera>();

		_playerCamera.fieldOfView = _deffaultFOV;
	}
}
