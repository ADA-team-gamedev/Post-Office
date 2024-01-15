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
	[SerializeField] private LayerMask _obstacleMask;

	public Vector2 MoveDirection { get; private set; }

	private Vector3 _velocityChange;

	private float _playerSpeed;

	public MovementState MovementState { get; private set; } = MovementState.Idle;

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

	private float _originalPlayerColliderHeight;

	private CapsuleCollider _playerCollider;

	private bool _isPlayerStandUp = true;

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

		_sprintCooldownReset = _sprintCooldown;

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

		if (!_isPlayerStandUp && CanPlayerStandUp() && MovementState != MovementState.Crouching)
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
		if (MovementState == MovementState.Sprinting && _sprintRemaining > 0)
		{
			if (_sprintRemaining > 0f && !_isSprintCooldown)
				_sprintBarCanvasGroup.alpha += 5 * Time.deltaTime;
			else if (_sprintRemaining == _sprintDuration)
				_sprintBarCanvasGroup.alpha -= 3 * Time.deltaTime;

			_sprintRemaining -= 1 * Time.deltaTime;
			
			if (_sprintRemaining <= 0)
			{
				MovementState = MovementState.Walking;
				
				_playerSpeed = _playerWalkSpeed;

				_isSprintCooldown = true;			
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
		{
			_sprintCooldown = _sprintCooldownReset;
		}

		_sprintBar.value = _sprintRemaining / _sprintDuration;
	}

	private void Crouch()
	{
		if (MovementState != MovementState.Crouching || !_isPlayerStandUp)
		{
			if (!_isPlayerStandUp && CanPlayerStandUp())
			{
				_isPlayerStandUp = true;

				_playerCollider.height = _originalPlayerColliderHeight;

				if (MovementState == MovementState.Walking || MovementState == MovementState.Idle) //For moment when we can't stand up, but we moved with not walking speed(sprint, as an example)
					_playerSpeed = _playerWalkSpeed;
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
			MovementState = MovementState.Walking;	
		}
		else if (context.canceled && MoveDirection == Vector2.zero)
		{
			if (MovementState != MovementState.Crouching)
				MovementState = MovementState.Idle;
		}
	}

	private void OnSprint(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			MovementState = MovementState.Sprinting;

			_playerSpeed = _playerSprintSpeed;
		}
		else if (context.canceled)
		{
			MovementState = MovementState.Walking;

			_playerSpeed = _playerWalkSpeed;
		}
	}

	private void OnCrouch(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			MovementState = MovementState.Crouching;

			_playerSpeed = _playerCrouchSpeed;
		}
		else if (context.canceled && CanPlayerStandUp())
		{
			MovementState = MovementState.Idle;

			_playerSpeed = _playerWalkSpeed;
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
