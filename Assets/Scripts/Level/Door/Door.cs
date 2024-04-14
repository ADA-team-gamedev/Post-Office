using UnityEngine;
using Items.Keys;
using Player;
using Audio;

namespace Level.Doors
{
	public enum DoorRotationDirection
	{
		Possitive = 1,
		Neutral = 0,
		Negative = -1,
	}

	[RequireComponent(typeof(HingeJoint))] //don't forget to set up them, change rigidbody to static
	public class Door : MonoBehaviour, IInteractable
	{
		#region Door rotation

		#region Parameters

		[Header("Values")]

		[SerializeField] private bool _isDoorMustBeClosedOnStart = true;

		[SerializeField] private float _doorDragingDistance = 3f;

		[SerializeField] private float _doorOpeningForce = 10f;
		[SerializeField, Range(2000f, 10000f)] private float _doorRotationSpeed = 5000f;

		[SerializeField, Range(0f, 90f)] private float _rotationDegressThreshold = 45f;

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

		private DoorRotationDirection _previousDoorRotationDirection = DoorRotationDirection.Neutral;

		private float _currentDegressThreshold;

		#endregion

		#region Key open

		[field: Header("Key Opening")]

		[field: SerializeField] public bool IsClosed { get; private set; } = true;

		[field: SerializeField] public DoorKeyTypes DoorKeyType { get; private set; }

		#endregion

		[Header("Sounds")]

		[SerializeField] private string _unlockDoorSound = "Unlock Door";
		[SerializeField] private string _closedDoor = "Door Closed";
		[SerializeField] private string _fullyClosedDoor = "Fully Closed Door";
		[SerializeField] private string _doorRotationSound = "Door Rotation";

		private bool _isPlayerDragDoor = false;

		private void Start()
		{
			_hingeJoint ??= GetComponent<HingeJoint>();

			_defaultDoorYRotation = transform.rotation.eulerAngles.y;

			_playerClickedViewPoint = _doorModel.position;

			_doorRotation = _isDoorMustBeClosedOnStart ? _hingeJoint.limits.max : _hingeJoint.limits.min;
			
			transform.rotation = Quaternion.Euler(0, _doorRotation, 0);

			_currentDegressThreshold = transform.rotation.eulerAngles.y;
		}

		private void Update()
		{
			TryRotateDoor();
		}

		#region Key open

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
				Debug.Log("Player doesn't have right key to open this door");
			}

			AudioManager.Instance.PlaySound(_closedDoor, transform.position, spatialBlend: 0.8f);
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

			float doorRotation = GetDoorRotation() * _doorRotationSpeed * Time.deltaTime;

			DoorRotationDirection currentDoorRotationDirection;

			if (doorRotation > 0) 
				currentDoorRotationDirection = DoorRotationDirection.Possitive;
			else if (doorRotation < 0)
				currentDoorRotationDirection = DoorRotationDirection.Negative;
			else
				currentDoorRotationDirection = DoorRotationDirection.Neutral;

			_doorRotation += Mathf.Clamp(-doorRotation, -_doorOpeningForce, _doorOpeningForce); //we must invert direction

			_doorRotation = Mathf.Clamp(_doorRotation, _hingeJoint.limits.min, _hingeJoint.limits.max);
			
			transform.rotation = Quaternion.Euler(0, _doorRotation, 0);
			
			if (currentDoorRotationDirection != DoorRotationDirection.Neutral && currentDoorRotationDirection != _previousDoorRotationDirection)
			{
				_previousDoorRotationDirection = currentDoorRotationDirection;

				if (transform.rotation.eulerAngles.y >= _currentDegressThreshold + _rotationDegressThreshold || transform.rotation.eulerAngles.y <= _currentDegressThreshold - _rotationDegressThreshold)
				{
					_currentDegressThreshold = transform.rotation.eulerAngles.y;
					
					AudioManager.Instance.PlaySound(_doorRotationSound, transform.position, spatialBlend: 0.8f);
				}
			}
		}

		private void StartRotateDoor()
		{
			if (IsClosed)
				return;

			_isPlayerDragDoor = true;

			_isDoorMoving = true;
		}

		private void StopRotateDoor()
		{
			if (!_isDoorMoving)
				return;
			
			if (Mathf.Approximately(transform.rotation.eulerAngles.y, _defaultDoorYRotation))
				AudioManager.Instance.PlaySound(_fullyClosedDoor, transform.position, spatialBlend: 0.8f);		

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

			if (IsClosed)
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
}