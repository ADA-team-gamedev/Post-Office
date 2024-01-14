using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
	#region Fields

	#region Camera Settings

	[Header("Camera Settigs")]

    [SerializeField] private Camera _playerCamera;

    [SerializeField] private float _cameraFOV = 60f;
	public bool CanCameraMove = true;
	[SerializeField] private bool _isCameraInverted = false;
    [SerializeField] private float _cameraSensitivity = 1f;
    [SerializeField] private float _maxCameraLookAngle = 60f;

    [SerializeField] private bool _isLockCursor = true;
    [SerializeField] private bool _isCrosshair = true;
	[SerializeField] private Image _crosshairImage;
	[SerializeField] private Color _crosshairColor = Color.white;

	// Internal Variables
	private float _yaw = 0.0f;
	private float _pitch = 0.0f;	

	#region Camera zoom

	[Header("Zoom")]

	[SerializeField] private bool _isZoomEnabled = true;
	[SerializeField] private bool _isNeededHoldToZoom = true;
	[SerializeField] private KeyCode _zoomKey = KeyCode.Mouse1;
	[SerializeField] private float _zoomFOV = 30f;
	[SerializeField] private float _zoomStepTime = 5f;

	private bool _isZoomed = false;

	#endregion

	#endregion

	#region Movement Settings

	[Header("Movement Settings")]

	public bool CanPlayerMove = true;

	[SerializeField] private float _playerWalkSpeed = 5f;
	[SerializeField] private float _maxVelocityChange = 10f;

	private bool _isWalking = false;
	private bool _isGrounded = false;

	#region Sprint 

	[Header("Sprint")]

	[SerializeField] private bool _isSprintEnabled = true;
	[SerializeField] private bool _isUnlimitedSprint = false;

	[SerializeField] private KeyCode _sprintKey = KeyCode.LeftShift;

	[SerializeField] private float _sprintSpeed = 10f;
	[SerializeField] private float _sprintDuration = 5f;
	[SerializeField] private float _sprintCooldown = 0.5f;

	[SerializeField] private float _sprintFOV = 80f;
	[SerializeField] private float _sprintFOVStepTime = 10f;

	[Space(10)]

	[SerializeField] private bool _hideSprintBarWhenFull = true;
	[SerializeField] private Slider _sprintBar;
	[SerializeField] private CanvasGroup _sprintBarCanvasGroup;

	private bool _isSprinting = false;
	private float _sprintRemaining;
	private bool _isSprintCooldown = false;
	private float _sprintCooldownReset;

	#endregion

	#region Crouch

	[Header("Crouch")]

	[SerializeField] private bool _isCrouchEnabled = true;
	[SerializeField] private bool _isNeedHoldToCrouch = true;

	[SerializeField] private KeyCode _crouchKey = KeyCode.LeftControl;

	[SerializeField][Range(0.5f, 1f)] private float _crouchPlayerYCoefficient = 0.8f;
	[SerializeField][Range(0.1f, 1f)] private float _crouchSpeedReduction = 0.5f;

	[SerializeField][Range(0.5f, 1f)] private float _maxCheckDistanceAbovePlayer = 0.9f;
	[SerializeField][Range(0.5f, 1f)] private float _checkerRadiusPart = 1f;

	private bool _isCrouched = false;

	private float _originalPlayerColliderY;

	private CapsuleCollider _playerCollider;

	#endregion

	#endregion

	#region Head Bob

	[Header("Head bob")]

	[SerializeField] private bool _isHeadBobEnabled = true;
	[SerializeField] private Transform _joint;
	[SerializeField] private float _bobSpeed = 10f;
	[SerializeField] private Vector3 _bobAmount = new(0f, 0.05f, 0f);

	private Vector3 _jointOriginalPosition;
	private float _timer = 0;

	#endregion

	private Rigidbody _rb;

	private Vector3 _velocityChange;

	private bool _isPLayerStandUp = true;

	#endregion

	private void Awake()
	{
		_originalPlayerColliderY = _playerCollider.height;

		_jointOriginalPosition = _joint.localPosition;

		if (!_isUnlimitedSprint)
		{
			_sprintRemaining = _sprintDuration;

			_sprintCooldownReset = _sprintCooldown;
		}
	}

	private void Start()
	{
		if (_isLockCursor)
			Cursor.lockState = CursorLockMode.Locked;

		if (_isCrosshair)
		{
			_crosshairImage.gameObject.SetActive(true);

			_crosshairImage.color = _crosshairColor;
		}
		else
			_crosshairImage.gameObject.SetActive(false);

		_sprintBarCanvasGroup.gameObject.SetActive(_isSprintEnabled);

		if (_hideSprintBarWhenFull)
			_sprintBarCanvasGroup.alpha = 0;
	}

	private void Update()
	{
		MovementHandleInput();

		CameraHandleInput();

		SprintHandleInput();

		CrouchHandleInput();

		CheckGround();

		if (_isHeadBobEnabled)
			HeadBob();
	}

	private void FixedUpdate()
	{
		_rb.AddForce(_velocityChange, ForceMode.VelocityChange);
	}

	#region Handles

	private void CameraHandleInput()
	{
		if (CanCameraMove)
		{
			_yaw = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * _cameraSensitivity;

			if (!_isCameraInverted)
				_pitch -= _cameraSensitivity * Input.GetAxis("Mouse Y");
			else
				_pitch += _cameraSensitivity * Input.GetAxis("Mouse Y");

			_pitch = Mathf.Clamp(_pitch, -_maxCameraLookAngle, _maxCameraLookAngle);

			transform.localEulerAngles = new(0, _yaw, 0);
			_playerCamera.transform.localEulerAngles = new(_pitch, 0, 0);
		}

		if (_isZoomEnabled)
		{
			if (Input.GetKeyDown(_zoomKey) && !_isNeededHoldToZoom && !_isSprinting)
			{
				_isZoomed = !_isZoomed ? true : false;
			}

			if (_isNeededHoldToZoom && !_isSprinting)
			{
				if (Input.GetKeyDown(_zoomKey))
					_isZoomed = true;		
				else if (Input.GetKeyUp(_zoomKey))
					_isZoomed = false;				
			}

			if (_isZoomed)
				_playerCamera.fieldOfView = Mathf.Lerp(_playerCamera.fieldOfView, _zoomFOV, _zoomStepTime * Time.deltaTime);
			else if (!_isZoomed && !_isSprinting)
				_playerCamera.fieldOfView = Mathf.Lerp(_playerCamera.fieldOfView, _cameraFOV, _zoomStepTime * Time.deltaTime);
		}
	}

	private void MovementHandleInput()
	{
		if (!CanPlayerMove)
			return;
		
		Vector3 targetVelocity = new(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

		if (targetVelocity.x != 0 || targetVelocity.z != 0 && _isGrounded)
			_isWalking = true;	
		else
			_isWalking = false;	

		Vector3 velocity = _rb.velocity;
		Vector3 velocityChange;

		//while sprinting
		if (_isSprintEnabled && Input.GetKey(_sprintKey) && _sprintRemaining > 0f && !_isSprintCooldown && _isWalking)
		{
			targetVelocity = transform.TransformDirection(targetVelocity) * _sprintSpeed;

			velocityChange = targetVelocity - velocity;

			velocityChange.x = Mathf.Clamp(velocityChange.x, -_maxVelocityChange, _maxVelocityChange);
			velocityChange.z = Mathf.Clamp(velocityChange.z, -_maxVelocityChange, _maxVelocityChange);
			velocityChange.y = 0;

			if (velocityChange.x != 0 || velocityChange.z != 0)
			{
				_isSprinting = true;

				if (_isCrouched)
					Crouch();
				
				if (_hideSprintBarWhenFull && !_isUnlimitedSprint)
					_sprintBarCanvasGroup.alpha += 5 * Time.deltaTime;
			}	
		}
		//while walking
		else
		{
			_isSprinting = false;

			if (_hideSprintBarWhenFull && _sprintRemaining == _sprintDuration)
				_sprintBarCanvasGroup.alpha -= 3 * Time.deltaTime;
			
			targetVelocity = transform.TransformDirection(targetVelocity) * _playerWalkSpeed;

			velocityChange = targetVelocity - velocity;

			velocityChange.x = Mathf.Clamp(velocityChange.x, -_maxVelocityChange, _maxVelocityChange);
			velocityChange.z = Mathf.Clamp(velocityChange.z, -_maxVelocityChange, _maxVelocityChange);
			velocityChange.y = 0;			
		}
	
		_velocityChange = velocityChange;
	}

	private void SprintHandleInput()
	{
		if (!_isSprintEnabled)
			return;	

		if (_isSprinting)
		{
			_isZoomed = false;

			_playerCamera.fieldOfView = Mathf.Lerp(_playerCamera.fieldOfView, _sprintFOV, _sprintFOVStepTime * Time.deltaTime);

			if (!_isUnlimitedSprint)
			{
				_sprintRemaining -= 1 * Time.deltaTime;

				if (_sprintRemaining <= 0)
				{
					_isSprinting = false;
					_isSprintCooldown = true;
				}
			}
		}
		else
		{
			_sprintRemaining = Mathf.Clamp(_sprintRemaining += 1 * Time.deltaTime, 0, _sprintDuration);
		}

		if (_isSprintCooldown)
		{
			_sprintCooldown -= 1 * Time.deltaTime;

			if (_sprintCooldown <= 0)
				_isSprintCooldown = false;
		}
		else
			_sprintCooldown = _sprintCooldownReset;

		if (!_isUnlimitedSprint)
			_sprintBar.value = _sprintRemaining / _sprintDuration;
	}

	private void CrouchHandleInput()
	{
		if (!_isCrouchEnabled)
			return;

		if (Input.GetKeyDown(_crouchKey) && !_isNeedHoldToCrouch && !_isCrouched)
			Crouch();

		if (Input.GetKeyDown(_crouchKey) && _isNeedHoldToCrouch && !_isCrouched)
		{
			_isCrouched = false;
			Crouch();
		}
		else if (Input.GetKeyUp(_crouchKey) && _isNeedHoldToCrouch && _isCrouched)
		{
			_isCrouched = true;
			Crouch();
		}

		if (_isCrouched && !_isPLayerStandUp)
		{
			if (!Physics.SphereCast(transform.position, _playerCollider.radius * _checkerRadiusPart, transform.transform.up, out RaycastHit _, _maxCheckDistanceAbovePlayer))
			{
				_isPLayerStandUp = true;
				
				Crouch();
			}
		}	
	}

	#endregion

	private void CheckGround()
	{
		Vector3 origin = new(transform.position.x, transform.position.y - (transform.localScale.y * 0.5f), transform.position.z);
		Vector3 direction = transform.TransformDirection(Vector3.down);

		float distance = 0.75f;

		if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
		{
			Debug.DrawRay(origin, direction * distance, Color.red);
			_isGrounded = true;
		}
		else
			_isGrounded = false;
	}

	private void Crouch()
	{
		if (_isCrouched)
		{
			if (!Physics.SphereCast(transform.position, _playerCollider.radius * _checkerRadiusPart, transform.transform.up, out RaycastHit _, _maxCheckDistanceAbovePlayer))
			{
				_isPLayerStandUp = true;

				_playerCollider.height = _originalPlayerColliderY;

				_playerWalkSpeed /= _crouchSpeedReduction;

				_isCrouched = false;
			}
			else
				_isPLayerStandUp = false;
		}
		else
		{
			_playerCollider.height *= _crouchPlayerYCoefficient;

			_playerWalkSpeed *= _crouchSpeedReduction;

			_isCrouched = true;
		}
	}

	private void HeadBob()
	{
		if (!_isWalking)
		{
			_timer = 0;
			_joint.localPosition = new(Mathf.Lerp(_joint.localPosition.x, _jointOriginalPosition.x, Time.deltaTime * _bobSpeed),
				Mathf.Lerp(_joint.localPosition.y, _jointOriginalPosition.y, Time.deltaTime * _bobSpeed),
				Mathf.Lerp(_joint.localPosition.z, _jointOriginalPosition.z, Time.deltaTime * _bobSpeed));

			return;
		}

		if (_isSprinting)
			_timer += Time.deltaTime * (_bobSpeed + _sprintSpeed);
		else if (_isCrouched)
			_timer += Time.deltaTime * (_bobSpeed * _crouchSpeedReduction);
		else
			_timer += Time.deltaTime * _bobSpeed;

		_joint.localPosition = new Vector3(_jointOriginalPosition.x + Mathf.Sin(_timer) * _bobAmount.x, _jointOriginalPosition.y + Mathf.Sin(_timer) * _bobAmount.y, _jointOriginalPosition.z + Mathf.Sin(_timer) * _bobAmount.z);
	}

	private void OnValidate()
	{
		_rb ??= GetComponent<Rigidbody>();

		_playerCollider ??= GetComponent<CapsuleCollider>();

		_playerCamera.fieldOfView = _cameraFOV;

		if (_sprintSpeed <= _playerWalkSpeed)
			_sprintSpeed = _playerWalkSpeed + 1;

		if (_sprintFOV <= _cameraFOV)
			_sprintFOV = _cameraFOV + 1;
	}
}
