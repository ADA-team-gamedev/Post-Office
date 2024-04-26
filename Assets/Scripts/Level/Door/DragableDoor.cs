using Audio;
using Items.Keys;
using Player;
using UnityEngine;

[RequireComponent(typeof(HingeJoint))]
public class DragableDoor : MonoBehaviour, IInteractable
{
	#region Door Rotation

	[Header("Door Rotation")]

	[SerializeField] private bool _isDoorMustBeClosedOnStart = true;

	[SerializeField] private Interactor _playerInteractor;

	[SerializeField] private Transform _doorCenter;

	[SerializeField] private float _doorDragingDistance = 2f;

	[SerializeField] private float _rotationSpeed = 1f;

	#endregion

	#region Key Open

	[field: Header("Key Open")]

	[field: SerializeField] public bool IsClosed { get; private set; } = true;

	[field: SerializeField] public DoorKeyTypes DoorKeyType { get; private set; }

	#endregion

	#region Sounds

	[Header("Sounds")]

	[SerializeField] private string _unlockDoorSound = "Unlock Door";
	[SerializeField] private string _closedDoor = "Door Closed";
	[SerializeField] private string _fullyClosedDoor = "Fully Closed Door";
	[SerializeField] private string _doorRotationSound = "Door Rotation";

	#endregion

	private Transform _interactorCameraTransform => _playerInteractor.PlayerCamera.transform;	

	#region Vectors

	private Vector3 _playerClickedViewPoint => _interactorCameraTransform.position + _interactorCameraTransform.forward * _doorDragingDistance;

	private Vector3 _newSpherePosition => new(_playerClickedViewPoint.x, transform.position.y, _playerClickedViewPoint.z);

	private Vector3 _pivotDirection => (new Vector3(transform.position.x, transform.position.y, transform.position.z + 1) - transform.position).normalized;

	private Vector3 _pivotRightDirection => (new Vector3(transform.position.x + 1, transform.position.y, transform.position.z) - transform.position).normalized;

	private Vector3 _sphereDirection => (_newSpherePosition - transform.position).normalized;

	#endregion

	private bool _canDoorRotate = false;

	private float _defaultYRotation;

	private float _angle;

	private HingeJoint _hingeJoint;

	private void Start()
	{
		_defaultYRotation = transform.rotation.eulerAngles.y;

		_hingeJoint = GetComponent<HingeJoint>();
		
		_angle = _isDoorMustBeClosedOnStart ? _hingeJoint.limits.max : _hingeJoint.limits.min;

		transform.rotation = Quaternion.Euler(0, _angle, 0);
	}

	#region Key Open

	private void TryOpenDoorByKey()
	{
		if (!IsClosed)
			return;

		bool hasRightKeyInInventory = PlayerInventory.Instance.TryGetCurrentItem(out Key key) && key.KeyType == DoorKeyType;

		bool hasRightKeyInKeyBunch = PlayerInventory.Instance.TryGetCurrentItem(out KeyBunch keyBunch) && keyBunch.IsContainsKey(DoorKeyType);

		if (hasRightKeyInInventory || hasRightKeyInKeyBunch)
		{
			AudioManager.Instance.PlaySound(_unlockDoorSound, transform.position, spatialBlend: 0.8f);

			IsClosed = false;

			return;
		}
		else
		{
#if UNITY_EDITOR
			Debug.Log("Player doesn't have right key to open this door");
#endif
		}

		AudioManager.Instance.PlaySound(_closedDoor, transform.position, spatialBlend: 0.8f);
	}

	#endregion

	#region Door Rotation

	private void RotateDoor()
	{
		if (!_canDoorRotate || !IsPlayerInInteractionZone())
		{
			StopRotateDoor();

			return;
		}

		CalculateAngle();

		Quaternion lookRotation = Quaternion.Euler(0, _angle, 0);

		Quaternion smoothRotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * _rotationSpeed);
		
		transform.rotation = smoothRotation;
	}

	private void CalculateAngle()
	{
		float angle = Vector3.SignedAngle(_pivotDirection, _sphereDirection, Vector3.up);

		if (angle < 0)
			angle += 360;

		float maxAngle = _hingeJoint.limits.max;
		float minAngle = _hingeJoint.limits.min;

		float anglesSum = minAngle + maxAngle;

		float circlePart = anglesSum / 360;

		float lastArea = (360 - 360 * circlePart) * 0.5f;

		if (angle < 360 && angle > maxAngle)
		{
			if (angle < maxAngle + lastArea)
				angle = maxAngle;
			else if (angle > maxAngle + lastArea)
				angle = minAngle;
		}	

		_angle = angle;	
	}

	private void StartRotateDoor()
	{
		if (IsClosed)
			return;

		_canDoorRotate = true;

		AudioManager.Instance.PlaySound(_doorRotationSound, transform.position, spatialBlend: 0.8f);
	}

	private void StopRotateDoor()
	{
		if (!_canDoorRotate)
			return;

		float currentRotationY = Mathf.Round(transform.rotation.eulerAngles.y);

		if (Mathf.Approximately(currentRotationY, _defaultYRotation))
			AudioManager.Instance.PlaySound(_fullyClosedDoor, transform.position, spatialBlend: 0.8f);

		_canDoorRotate = false;
	}

	private bool IsPlayerInInteractionZone()
		=> Vector3.Magnitude(_interactorCameraTransform.position - _doorCenter.position) <= _playerInteractor.InteractionDistance;

	#endregion

	#region Interaction

	public void StartInteract()
	{
		TryOpenDoorByKey();

		StartRotateDoor();
	}

	public void UpdateInteract()
	{
		RotateDoor();
	}

	public void StopInteract()
	{
		StopRotateDoor();
	}

	#endregion

	private void OnValidate()
	{
		_hingeJoint ??= GetComponent<HingeJoint>();
	}

	#region Gizmos

	private void OnDrawGizmos()
	{
		DrawAxis();

		DrawAngleLine();

		DrawLimitLines();

		DrawPlayerInteractionLine();

		DrawInteractionDisctanceLine();
	}

	private void DrawAxis()
	{
		Gizmos.color = Color.blue;

		Gizmos.DrawWireSphere(transform.position, _pivotDirection.sqrMagnitude);

		//y line
		Gizmos.color = Color.yellow;

		Gizmos.DrawRay(transform.position, _pivotDirection);
		Gizmos.DrawRay(transform.position, -_pivotDirection);

		//x line
		Gizmos.color = Color.yellow;

		Gizmos.DrawRay(transform.position, _pivotRightDirection);
		Gizmos.DrawRay(transform.position, -_pivotRightDirection);
	}

	private void DrawAngleLine()
	{
		if (!_playerInteractor)
			return;

		Gizmos.color = Color.red;

		Gizmos.DrawRay(transform.position, _sphereDirection);
	}

	private void DrawLimitLines()
	{
		//min limit line
		Gizmos.color = Color.green;

		Vector3 minLimit = Quaternion.Euler(0, _hingeJoint.limits.min, 0) * _pivotDirection;

		Vector3 maxLimit = Quaternion.Euler(0, _hingeJoint.limits.max, 0) * _pivotDirection;

		Gizmos.DrawRay(transform.position, minLimit);

		Gizmos.DrawRay(transform.position, maxLimit);
	}

	private void DrawPlayerInteractionLine()
	{
		if (!_playerInteractor)
			return;

		Gizmos.color = Color.red;

		Gizmos.DrawSphere(_playerClickedViewPoint, 0.1f);

		Gizmos.color = Color.blue;
		Gizmos.DrawRay(_interactorCameraTransform.position, _interactorCameraTransform.forward * _doorDragingDistance);
	}

	private void DrawInteractionDisctanceLine()
	{
		if (!_doorCenter || !_playerInteractor)
			return;

		Gizmos.color = IsPlayerInInteractionZone() ? Color.green : Color.red; 
		Gizmos.DrawLine(_interactorCameraTransform.position, _doorCenter.position);
	}

	#endregion
}