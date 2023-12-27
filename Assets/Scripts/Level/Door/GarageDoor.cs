using UnityEngine;

public class GarageDoor : MonoBehaviour
{
	#region Door rotation
	[Header("Layer")]
	[SerializeField] private LayerMask _doorLayer;

	#region Parameters

	[Header("Values")]

	[SerializeField] private float _doorMaxHeight;

	[SerializeField] private float _doorDragingDistance = 3f;

	[SerializeField][Range(1, 10)] private float _doorRaisingSpeed = 5;

	#endregion

	[Header("Objects")]

	[SerializeField] private Camera _playerCamera;
	[SerializeField] private Transform _doorModel;

	[SerializeField] private GameObject[] _interactionButtonUI;

	private Vector3 _playerClickedViewPoint;

	private float _defaultDoorYPosition;
	private float _doorPosition;

	private bool _isDoorMoving = false;

	#endregion

	#region Key open

	[field: Header("Key Opening")]

	[field: SerializeField] public bool IsKeyNeeded { get; private set; } = true;

	[field: SerializeField] public DoorKeyTypes DoorKeyType { get; private set; }


	private bool _isClosed; //closed while we don't open it by our key

	#endregion

	private void Start()
	{
		PlayerHandler.Instance.KeyBinds[InputType.Interact].OnKeyDown += OpenDoorByKey;

		PlayerHandler.Instance.KeyBinds[InputType.Drag].OnKeyDown += StartRotateDoor;
		PlayerHandler.Instance.KeyBinds[InputType.Drag].OnKeyUp += StopRotateDoor;

		_defaultDoorYPosition = _doorModel.position.y;

		_playerClickedViewPoint = _doorModel.position;

		_isClosed = IsKeyNeeded;

		foreach (GameObject button in _interactionButtonUI)
			button.SetActive(false);
	}

	private void Update()
	{
		TryRaiseDoor();

		ShowInteractionUI();
	}

	#region Key open

	private void ShowInteractionUI()
	{
		if (!IsKeyNeeded || !IsPlayerInInteractionZone())
		{
			foreach (GameObject button in _interactionButtonUI)
				button.SetActive(false);

			return;
		}

		foreach (GameObject button in _interactionButtonUI)
			button.SetActive(true);

		for (int i = 0; i < _interactionButtonUI.Length; i++)
			_interactionButtonUI[i].transform.rotation = Quaternion.LookRotation(_interactionButtonUI[i].transform.position - _playerCamera.transform.position);
	}

	private void OpenDoorByKey()
	{
		if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out RaycastHit hit, _doorDragingDistance, _doorLayer))
		{
			if (PlayerInventory.Instance.TryGetCurrentItem(out GameObject item) && item.TryGetComponent(out Key key))
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
	}

	#endregion

	#region Door raising
	private void TryRaiseDoor()
	{
		if (_isClosed || !IsPlayerInInteractionZone())
		{
			_isDoorMoving = false;

			_playerClickedViewPoint = _doorModel.position;

			return;
		}

		if (_isDoorMoving)
			_playerClickedViewPoint = _playerCamera.transform.position + _playerCamera.transform.forward * _doorDragingDistance;	

		if (_isDoorMoving) //the code below must work only if door moving 
		{
			_doorPosition = Mathf.Lerp(_doorPosition, _playerClickedViewPoint.y, _doorRaisingSpeed * Time.deltaTime);

			_doorPosition = Mathf.Clamp(_doorPosition, _defaultDoorYPosition, _doorMaxHeight);

			_doorModel.position = new(_doorModel.position.x, _doorPosition, _doorModel.position.z);
		}
	}

	private void StartRotateDoor()
	{
		if (_isClosed || !IsPlayerInInteractionZone())
		{
			_isDoorMoving = false;

			_playerClickedViewPoint = _doorModel.position;

			return;
		}

		if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out RaycastHit hit, _doorDragingDistance, _doorLayer))
			_isDoorMoving = true;
	}

	private void StopRotateDoor()
	{
		if (!_isDoorMoving)
			return;

		if (_doorModel.position.y == _defaultDoorYPosition)
		{
			//play fully closed door sound
		}

		_isDoorMoving = false;
	}

	#endregion

	private bool IsPlayerInInteractionZone()
		=> Vector3.Magnitude(_playerCamera.transform.position - _doorModel.position) <= _doorDragingDistance;

	private void OnValidate()
	{
		_defaultDoorYPosition = _doorModel.position.y;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;

		Gizmos.DrawSphere(_playerClickedViewPoint, 0.1f);

		Gizmos.color = Color.blue;
		Gizmos.DrawRay(_playerCamera.transform.position, _playerCamera.transform.forward * _doorDragingDistance);

		Gizmos.color = IsPlayerInInteractionZone() ? Color.green : Color.red; //interaction zone; is player can rotate door
		Gizmos.DrawLine(_playerCamera.transform.position, _doorModel.position);

		//door possible max position
		Gizmos.color = Color.cyan;

		Gizmos.DrawWireCube(new(_doorModel.position.x, _doorMaxHeight, _doorModel.position.z), _doorModel.localScale);

		//door possible min position
		Gizmos.color = Color.green;

		Gizmos.DrawWireCube(new(_doorModel.position.x, _defaultDoorYPosition, _doorModel.position.z), _doorModel.localScale);
	}
}
