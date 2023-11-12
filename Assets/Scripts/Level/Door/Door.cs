using UnityEngine;

[RequireComponent(typeof(HingeJoint))] //don't forget to set up them, change rigidbody to static
public class Door : MonoBehaviour
{
	#region Door rotation
	[Header("Layer")]
	[SerializeField] private LayerMask _doorLayer;

	#region Parameters

	[Header("Values")]

	[SerializeField] private float _doorDragingDistance = 3f;

	[SerializeField] private float _doorOpeningForce = 10f;
	[SerializeField][Range(2000f, 10000f)] private float _doorRotationSpeed = 5000f;

	#endregion

	[Header("Objects")]

	[SerializeField] private Camera _playerCamera;
	[SerializeField] private Transform _doorModel;

	private HingeJoint _hingeJoint;

	private Vector3 _playerClickedViewPoint;

	private float _doorRotation;

	private bool _isDoorMoving = false;

	#endregion

	#region Key open

	[field: Header("Key Opening")]

	[field: SerializeField] public bool IsKeyNeeded { get; private set; } = true;

	[SerializeField] private KeyCode _doorOpenKey = KeyCode.E;
	[field: SerializeField] public DoorKeyTypes DoorKeyType { get; private set; }


	private bool _isClosed; //closed while we don't open it by our key

	#endregion 

	private void Start()
    {
        _playerClickedViewPoint = _doorModel.position;

		_isClosed = IsKeyNeeded;
	}
   
    private void Update()
    {
		TryRotateDoor();

		TryOpenDoorByKey();
	}

	#region Key open

	public void TryOpenDoorByKey()
	{
		if (!IsKeyNeeded || !IsPlayerInInteractionZone())
			return;

		if (Input.GetKeyDown(_doorOpenKey))
		{
			if (KeyHolder.ContainsKey(DoorKeyType))
			{
				//play door opening sound

				IsKeyNeeded = false;

				_isClosed = false;
			}
			else
			{
				Debug.Log("Player doesn't have right key to open this door");

				//play door closed sound
			}
		}	
	}

	#endregion

	#region Door rotation
	private void TryRotateDoor()
	{
		if (_isClosed || !IsPlayerInInteractionZone())
		{
			_isDoorMoving = false;

			return;
		}

		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out RaycastHit hit, _doorDragingDistance, _doorLayer))
				_isDoorMoving = true;				
		}

		if (_isDoorMoving)
		{
			if (Input.GetKeyUp(KeyCode.Mouse0))
				_isDoorMoving = false;			

			_playerClickedViewPoint = _playerCamera.transform.position + _playerCamera.transform.forward * _doorDragingDistance;		
		}

		_doorRotation += Mathf.Clamp(-GetDoorRotation() * _doorRotationSpeed * Time.deltaTime, -_doorOpeningForce, _doorOpeningForce);

		_doorRotation = Mathf.Clamp(_doorRotation, _hingeJoint.limits.min, _hingeJoint.limits.max);

		transform.rotation = Quaternion.Euler(0, _doorRotation, 0);
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

	private bool IsPlayerInInteractionZone()
		=> Vector3.Distance(_playerCamera.transform.position, _doorModel.position) <= _doorDragingDistance;

	private void OnValidate()
	{
		_hingeJoint ??= GetComponent<HingeJoint>();
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;

		Gizmos.DrawSphere(_playerClickedViewPoint, 0.1f);

		Gizmos.color = Color.blue;
		Gizmos.DrawRay(_playerCamera.transform.position, _playerCamera.transform.forward * _doorDragingDistance);

		Gizmos.color = IsPlayerInInteractionZone() ? Color.green : Color.red; //interaction zone; is player can rotate door
		Gizmos.DrawLine(_playerCamera.transform.position, _doorModel.position);
	}
}