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
	#region Basic movement

	[Header("Movement")]

	[SerializeField] private float _playerWalkSpeed = 7f;

	[SerializeField] private float _groundDrag = 1f;

	[SerializeField] private LayerMask _groundMask;
	[SerializeField] private LayerMask _obstacleMask;

	public Vector2 MoveDirection { get; private set; }

	private Vector3 _velocityChange;

	private float _playerSpeed;

	public MovementState MovementState { get; private set; } = MovementState.Idle;

	#endregion

	#region Sprint 

	[Header("Sprint")]

	[SerializeField] private float _playerSprintSpeed = 12f;
	[SerializeField] private float _sprintDuration = 5f;
	[SerializeField] private float _sprintCooldownDelay = 1f;

	[Space(10)]

	[SerializeField] private Slider _sprintBar;
	[SerializeField] private CanvasGroup _sprintBarCanvasGroup;

	private float _sprintRemaining;
	private float _sprintCooldown;

	private bool _isSprintOnCooldown = false;

	#endregion

	#region Crouch

	[Header("Crouch")]

	[SerializeField][Range(0.5f, 1f)] private float _crouchPlayerHeightPercent = 0.8f;
	[SerializeField] private float _playerCrouchSpeed = 5;

	private float _originalPlayerColliderHeight;

	private CapsuleCollider _playerCollider;

	private bool _isPlayerStandUp = true;
	private bool _isTryingToStandUp = false;

	#endregion

	#region Head Bob

	[field: Header("Head bob")]

	[field: SerializeField] public bool HeadBobEnabled { get; private set; } = true;

	[SerializeField] private float _bobSpeed = 10f;
	[SerializeField] private Vector3 _bobAmount = new(0f, 0.05f, 0f);

	[SerializeField] private Transform _joint;

	private Vector3 _jointOriginalPosition;
	private float _timer = 0;

	#endregion

	private PlayerInput _playerInput;

	private Rigidbody _rb;

	private void Awake()
	{
		_playerSpeed = _playerWalkSpeed;

		_sprintRemaining = _sprintDuration;

		_sprintCooldown = _sprintCooldownDelay;

		_originalPlayerColliderHeight = _playerCollider.height;

		_jointOriginalPosition = _joint.localPosition;

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

		if (_isTryingToStandUp)
			Crouch();

		if (HeadBobEnabled)
			HeadBob();
	}

	private void Move()
	{
		if (MovementState == MovementState.Idle)
			return;

		Vector3 targetVelocity = new Vector3(MoveDirection.x, 0, MoveDirection.y) * _playerSpeed;

		targetVelocity = transform.TransformDirection(targetVelocity);

		Vector3 velocityChange = targetVelocity - _rb.velocity;

		_velocityChange = velocityChange;

		_rb.AddForce(_velocityChange);
	}

	private void Sprint()
	{
		_sprintBar.value = _sprintRemaining / _sprintDuration;

		if (_isSprintOnCooldown || MovementState != MovementState.Sprinting)
		{
			_sprintRemaining = Mathf.Clamp(_sprintRemaining += Time.deltaTime, 0, _sprintDuration);

			if (_sprintRemaining == _sprintDuration)
				_sprintBarCanvasGroup.alpha -= 3 * Time.deltaTime;

			if (_isSprintOnCooldown)
			{
				_playerSpeed = _playerCrouchSpeed;

				_sprintCooldown -= Time.deltaTime;

				if (_sprintCooldown <= 0)
				{
					_playerSpeed = _playerWalkSpeed;

					_isSprintOnCooldown = false;
				}
			}
			else
			{
				_sprintCooldown = _sprintCooldownDelay;
			}

			return;
		}

		_sprintBarCanvasGroup.alpha += 5 * Time.deltaTime;

		_sprintRemaining -= Time.deltaTime;

		if (_sprintRemaining <= 0)
		{
			_playerSpeed = _playerWalkSpeed;

			_isSprintOnCooldown = true;

			MovementState = MovementState.Walking;
		}
	}

	private void Crouch()
	{
		if (MovementState != MovementState.Crouching || _isTryingToStandUp)
		{
			if (!_isPlayerStandUp && CanPlayerStandUp())
			{
				_isPlayerStandUp = true;

				_isTryingToStandUp = false;

				_playerCollider.height = _originalPlayerColliderHeight;

				_playerSpeed = _playerWalkSpeed;

				if (MoveDirection == Vector2.zero)
					MovementState = MovementState.Idle;
				else if (_playerInput.Player.Sprint.IsPressed())
				{
					MovementState = MovementState.Sprinting;

					_playerSpeed = _playerSprintSpeed;
				}
				else
					MovementState = MovementState.Walking;
			}

			return;
		}

		_isPlayerStandUp = false;

		float crouchPlayerHeight = _originalPlayerColliderHeight * _crouchPlayerHeightPercent;

		_playerCollider.height = crouchPlayerHeight;

		_rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
	}

	private void HeadBob()
	{
		float x, y, z;

		if (MovementState == MovementState.Idle || MoveDirection == Vector2.zero) // For moments, when we aren't moving, but anyway it's not idle state(crouch idle, as an example)
		{
			_timer = 0;

			x = Mathf.Lerp(_joint.localPosition.x, _jointOriginalPosition.x, Time.deltaTime * _bobSpeed);
			y = Mathf.Lerp(_joint.localPosition.y, _jointOriginalPosition.y, Time.deltaTime * _bobSpeed);
			z = Mathf.Lerp(_joint.localPosition.z, _jointOriginalPosition.z, Time.deltaTime * _bobSpeed);

			_joint.localPosition = new(x, y, z);

			return;
		}

		float playerSpeedPercent = _playerSpeed / _playerWalkSpeed;

		_timer += Time.deltaTime * _bobSpeed * playerSpeedPercent;

		x = _jointOriginalPosition.x + Mathf.Sin(_timer) * _bobAmount.x;
		y = _jointOriginalPosition.y + Mathf.Sin(_timer) * _bobAmount.y;
		z = _jointOriginalPosition.z + Mathf.Sin(_timer) * _bobAmount.z;

		_joint.localPosition = new Vector3(x, y, z);
	}

	#region Chekers

	private bool CanPlayerStandUp()
	{
		Vector3 playerCurrentHeadPoint = new(transform.position.x, _playerCollider.height - _playerCollider.radius, transform.position.z);

		Vector3 playerOriginalHeadPoint = new(transform.position.x, _originalPlayerColliderHeight - _playerCollider.radius, transform.position.z);

		float maxDistance = Vector3.Distance(playerCurrentHeadPoint, playerOriginalHeadPoint);

		if (Physics.SphereCast(playerCurrentHeadPoint, _playerCollider.radius, transform.transform.up, out RaycastHit _, maxDistance, _obstacleMask))
			return false;

		return true;
	}

	private void CheckGround()
	{
		float playerHalfHeight = transform.localScale.y * 0.5f;

		Vector3 playerBodyCenter = new(transform.position.x, playerHalfHeight, transform.position.z);

		bool isGrounded = Physics.Raycast(playerBodyCenter, Vector3.down, playerHalfHeight + 0.2f, _groundMask);

		_rb.drag = isGrounded ? _groundDrag : 0;
	}

	#endregion

	#region Input entry points

	private void OnMove(InputAction.CallbackContext context)
	{
		MoveDirection = context.action.ReadValue<Vector2>();

		if (context.performed && MoveDirection != Vector2.zero && MovementState == MovementState.Idle)
		{
			if (_playerInput.Player.Sprint.IsPressed() && !_isSprintOnCooldown)
			{
				_playerSpeed = _playerSprintSpeed;

				MovementState = MovementState.Sprinting;
			}
			else
				MovementState = MovementState.Walking;
		}
		else if (context.canceled && MoveDirection == Vector2.zero)
		{
			if (MovementState == MovementState.Sprinting)
			{
				_playerSpeed = _playerWalkSpeed;

				MovementState = MovementState.Idle;
			}

			if (MovementState != MovementState.Crouching)
				MovementState = MovementState.Idle;
		}
	}

	private void OnSprint(InputAction.CallbackContext context)
	{
		if (context.performed && MovementState != MovementState.Idle && MovementState != MovementState.Crouching && !_isSprintOnCooldown)
		{
			MovementState = MovementState.Sprinting;

			_playerSpeed = _playerSprintSpeed;
		}
		else if (context.canceled)
		{
			_playerSpeed = _playerWalkSpeed;

			if (MoveDirection == Vector2.zero)
				MovementState = MovementState.Idle;
			else
				MovementState = MovementState.Walking;
		}
	}

	private void OnCrouch(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			MovementState = MovementState.Crouching;

			_playerSpeed = _playerCrouchSpeed;
		}
		else if (context.canceled)
		{
			_isTryingToStandUp = true;

			if (CanPlayerStandUp())
			{
				MovementState = MovementState.Idle;

				_playerSpeed = _playerWalkSpeed;
			}
		}

		Crouch();
	}

	#endregion

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
