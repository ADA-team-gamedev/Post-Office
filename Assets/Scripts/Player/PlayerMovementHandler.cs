using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum MovementState
{
	Idle,
	Walking,
	Sprinting,
	Crouching,
}
public class PlayerMovementHandler : MonoBehaviour
{
	[Header("Movement")]

	[SerializeField] private float _playerWalkSpeed = 7f;

	[SerializeField] private float _groundDrag = 1f;

	[SerializeField] private LayerMask _groundMask;

	private Vector2 _moveDirection;

	private Vector3 _velocityChange;

	private float _playerSpeed;

	private MovementState _movementState = MovementState.Idle;

	#region Sprint 

	[Header("Sprint")]

	[SerializeField] private float _playerSprintSpeed = 12f;
	[SerializeField] private float _sprintDuration = 5f;
	[SerializeField] private float _sprintCooldown = 0.5f;

	[Space(10)]

	[SerializeField] private Slider _sprintBar;
	[SerializeField] private CanvasGroup _sprintBarCanvasGroup;

	private float _sprintRemaining;
	private bool _isSprintCooldown = false;
	private float _sprintCooldownReset;

	#endregion

	#region Crouch

	[Header("Crouch")]

	[SerializeField][Range(0.5f, 1f)] private float _crouchPlayerHeightPercent = 0.8f;
	[SerializeField] private float _playerCrouchSpeed = 5;

	//[SerializeField][Range(0.5f, 1f)] private float _maxCheckDistanceAbovePlayer = 0.9f;
	//[SerializeField][Range(0.5f, 1f)] private float _checkerRadiusPart = 1f;

	private float _originalPlayerColliderHeight;

	private CapsuleCollider _playerCollider;

	#endregion

	#region Head Bob

	//[Header("Head bob")]

	//[SerializeField] private bool _isHeadBobEnabled = true;
	//[SerializeField] private Transform _joint;
	//[SerializeField] private float _bobSpeed = 10f;
	//[SerializeField] private Vector3 _bobAmount = new(0f, 0.05f, 0f);

	//private Vector3 _jointOriginalPosition;
	//private float _timer = 0;

	#endregion

	private PlayerInput _playerInput;

	private Rigidbody _rb;

	private void Awake()
	{
		_playerSpeed = _playerWalkSpeed;

		_sprintRemaining = _sprintDuration;

		_sprintCooldownReset = _sprintCooldown;

		_originalPlayerColliderHeight = _playerCollider.height;

		_playerInput = new();

		_playerInput.Player.Move.performed += OnMove;
		_playerInput.Player.Move.canceled += OnMove;

		_playerInput.Player.Sprint.performed += OnSprint;
		_playerInput.Player.Sprint.canceled += OnSprint;

		_playerInput.Player.Crouch.performed += OnCrouch;
		_playerInput.Player.Crouch.canceled += OnCrouch;
	}

	private void Start()
	{
		_sprintBarCanvasGroup.gameObject.SetActive(true);

		_sprintBarCanvasGroup.alpha = 0;
	}

	private void Update()
	{
		CheckGround();

		Move();

		Sprint();
	}

	private void Move()
	{
		if (_movementState == MovementState.Idle)
			return;

		Vector3 targetVelocity = new Vector3(_moveDirection.x, 0, _moveDirection.y) * _playerSpeed;

		targetVelocity = transform.TransformDirection(targetVelocity);

		Vector3 velocityChange = targetVelocity - _rb.velocity;

		_velocityChange = velocityChange;

		_rb.AddForce(_velocityChange);
	}

	private void Crouch()
	{
		if (_movementState != MovementState.Crouching)
		{
			_playerCollider.height = _originalPlayerColliderHeight;

			return;
		}

		_playerCollider.height *= _crouchPlayerHeightPercent;

		_rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);  
	}

	private void Sprint()
	{
		if (_movementState == MovementState.Sprinting)
		{
			if (_sprintRemaining > 0f && !_isSprintCooldown)
				_sprintBarCanvasGroup.alpha += 5 * Time.deltaTime;
			else if (_sprintRemaining == _sprintDuration)
				_sprintBarCanvasGroup.alpha -= 3 * Time.deltaTime;

			_sprintRemaining -= 1 * Time.deltaTime;
			
			if (_sprintRemaining <= 0)
				_isSprintCooldown = true;			
		}
		else
			_sprintRemaining = Mathf.Clamp(_sprintRemaining += 1 * Time.deltaTime, 0, _sprintDuration);	

		if (_isSprintCooldown)
		{
			_sprintCooldown -= 1 * Time.deltaTime;

			if (_sprintCooldown <= 0)
				_isSprintCooldown = false;
		}
		else
			_sprintCooldown = _sprintCooldownReset;

		_sprintBar.value = _sprintRemaining / _sprintDuration;
	}

	private void CheckGround()
	{
		float playerHalfHeight = transform.localScale.y * 0.5f;

		Vector3 playerBodyCenter = new(transform.position.x, playerHalfHeight, transform.position.z);

		bool isGrounded = Physics.Raycast(playerBodyCenter, Vector3.down, playerHalfHeight + 0.2f, _groundMask);

		_rb.drag = isGrounded ? _groundDrag : 0;
	}

	private void OnCrouch(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			_movementState = MovementState.Crouching;

			_playerSpeed = _playerCrouchSpeed;
		}
		else if (context.canceled)
		{
			_movementState = MovementState.Walking;

			_playerSpeed = _playerWalkSpeed;
		}

		Crouch();
	}

	private void OnSprint(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			_movementState = MovementState.Sprinting;

			_playerSpeed = _playerSprintSpeed;
		}
		else if (context.canceled)
		{
			_movementState = MovementState.Walking;

			_playerSpeed = _playerWalkSpeed;
		}
	}

	private void OnMove(InputAction.CallbackContext context)
	{
		_moveDirection = context.action.ReadValue<Vector2>();

		if (context.performed)
		{
			_movementState = MovementState.Walking;
		}
		else if (context.canceled)
			_movementState = MovementState.Idle;
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
		_rb ??= GetComponent<Rigidbody>();

		_playerCollider ??= GetComponent<CapsuleCollider>();
	}
}
