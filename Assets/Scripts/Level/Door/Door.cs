using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
	[SerializeField] private Camera _playerCamera;
    [SerializeField] private Transform _distanceChecker;

	[SerializeField] private float _doorOpeningForce = 10f;
	[SerializeField] private float _doorDragingDistance = 3f;

	[SerializeField] private Transform _doorHinge;

    [SerializeField] private Vector2 _rotationConstraits;

    private bool _isDoorMoving = false;
    private float _doorRotation;
    private Vector3 _targetPosition;

	
	private void Start()
    {
        _targetPosition = _distanceChecker.position;

	}

   
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out RaycastHit hit, _doorDragingDistance))
            {
                if (hit.collider.CompareTag("Door"))
                {
                    _isDoorMoving = true;
                }
            }
        }

        if (_isDoorMoving)
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                _isDoorMoving = false;
            }

            _targetPosition = _playerCamera.transform.position + _playerCamera.transform.forward * 2f;
        }

        _doorRotation += Mathf.Clamp(-GetDoorRotation() * 5000 * Time.deltaTime, -_doorOpeningForce, _doorOpeningForce);

        _doorRotation = Mathf.Clamp(_doorRotation, _rotationConstraits.x, _rotationConstraits.y);

        _doorHinge.transform.rotation = Quaternion.Euler(0, _doorRotation,0);
    }

    private float GetDoorRotation()
    {
        float firstDistance = (_distanceChecker.position - _targetPosition).sqrMagnitude;

        _doorHinge.transform.Rotate(Vector3.up);

        float secondDistance = (_distanceChecker.position - _targetPosition).sqrMagnitude;

        _doorHinge.transform.Rotate(-Vector3.up);

        return secondDistance - firstDistance; 

	}
}
