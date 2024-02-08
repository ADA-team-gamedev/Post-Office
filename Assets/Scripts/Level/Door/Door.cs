using UnityEngine;

[RequireComponent(typeof(HingeJoint))] //don't forget to set up them, change rigidbody to static
public class Door : MonoBehaviour, IInteractable
{
	#region Door rotation

	#region Parameters

	[Header("Values")]

	[SerializeField] private float _doorDragingDistance = 3f;

	[SerializeField] private float _doorOpeningForce = 10f;
	[SerializeField][Range(2000f, 10000f)] private float _doorRotationSpeed = 5000f;

	#endregion

	[Header("Objects")]

	[SerializeField] private Transform _doorModel;

	[SerializeField] private Interactor _playerInteractor;
	private Transform _interactorCameraTransform => _playerInteractor.PlayerCamera.transform;

	private HingeJoint _hingeJoint;

	private Vector3 _playerClickedViewPoint;

	private float _defaultDoorYRotation;
	private float _doorRotation;

	private bool _isDoorMoving = false;

	#endregion

	#region Key open

	[field: Header("Key Opening")]

	[field: SerializeField] public bool IsKeyNeeded { get; private set; } = true;

	[field: SerializeField] public DoorKeyTypes DoorKeyType { get; private set; }


	private bool _isClosed; //closed while we don't open it by our key

	#endregion

	private PlayerInput _playerInput;

	private bool _isPlayerDragDoor = false;

	private void Awake()
	{
		_playerInput = new();
	}

	private void Start()
	{
		_hingeJoint ??= GetComponent<HingeJoint>();

		_defaultDoorYRotation = transform.rotation.eulerAngles.y;

		_playerClickedViewPoint = _doorModel.position;

		_isClosed = IsKeyNeeded;

		_doorRotation = Mathf.Clamp(_doorRotation, _hingeJoint.limits.min, _hingeJoint.limits.max);

		transform.rotation = Quaternion.Euler(0, _doorRotation, 0);
	}

	private void Update()
	{
		TryRotateDoor();
	}

	#region Key open

	private void TryOpenDoorByKey()
	{
		if (!_isClosed)
			return;

		if (PlayerInventory.Instance.TryGetCurrentItem(out Key key))
		{
			if (key.DoorKeyType == DoorKeyType)
			{
				//play door key opening sound

				IsKeyNeeded = false;

				_isClosed = false;
			}
			else
			{
				Debug.Log("Player doesn't have right key to open this door");

				//play door key closed sound
			}
		}
	}

	#endregion

	#region Door rotation

	private void TryRotateDoor()
	{
		if (!_isDoorMoving || !IsPlayerInInteractionZone())
		{
			StopRotateDoor();

			return;
		}
	
		_playerClickedViewPoint = _interactorCameraTransform.position + _interactorCameraTransform.forward * _doorDragingDistance;

		_doorRotation += Mathf.Clamp(-GetDoorRotation() * _doorRotationSpeed * Time.deltaTime, -_doorOpeningForce, _doorOpeningForce);

		_doorRotation = Mathf.Clamp(_doorRotation, _hingeJoint.limits.min, _hingeJoint.limits.max);

		transform.rotation = Quaternion.Euler(0, _doorRotation, 0);
	}

	private void StartRotateDoor()
	{
		if (_isClosed)
			return;

		_isPlayerDragDoor = true;

		_isDoorMoving = true;
	}

	private void StopRotateDoor()
	{
		if (!_isDoorMoving)
			return;

		if (transform.rotation.eulerAngles.y == _defaultDoorYRotation)
		{
			//play fully closed door sound
		}

		_isPlayerDragDoor = false;

		_isDoorMoving = false;

		_playerClickedViewPoint = transform.position;
	}

	private float GetDoorRotation()
	{
		float firstDistance = (_doorModel.position - _playerClickedViewPoint).sqrMagnitude;

		transform.Rotate(Vector3.up);

		float secondDistance = (_doorModel.position - _playerClickedViewPoint).sqrMagnitude;

		transform.Rotate(-Vector3.up);

		return secondDistance - firstDistance;
	}

	#endregion

	public void StartInteract()
	{
		if (_isPlayerDragDoor)
			return;

		if (_isClosed)
			TryOpenDoorByKey();

		StartRotateDoor();
	}

	public void StopInteract()
	{
		StopRotateDoor();
	}

	private bool IsPlayerInInteractionZone()
		=> Vector3.Magnitude(_interactorCameraTransform.position - _doorModel.position) <= _playerInteractor.InteractionDistance;

	private void OnValidate()
	{
		_hingeJoint ??= GetComponent<HingeJoint>();
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
		Gizmos.color = Color.red;

		Gizmos.DrawSphere(_playerClickedViewPoint, 0.1f);

		Gizmos.color = Color.blue;
		Gizmos.DrawRay(_interactorCameraTransform.position, _interactorCameraTransform.forward * _doorDragingDistance);

		Gizmos.color = IsPlayerInInteractionZone() ? Color.green : Color.red; //interaction zone; is player can rotate door
		Gizmos.DrawLine(_interactorCameraTransform.position, _doorModel.position);
	}
}