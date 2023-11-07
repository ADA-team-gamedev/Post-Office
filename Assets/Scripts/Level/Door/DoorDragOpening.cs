using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorDragOpening : MonoBehaviour
{
	[SerializeField] private Camera _cam;
	[SerializeField] private float _doorOpeningForce = 10f;
	[SerializeField] private float _doorDragingDistance = 10f;

	private Transform selectedDoor;
	private GameObject dragPointGameobject;

	int leftDoor = 0;

	[SerializeField] private LayerMask doorLayer;

	[SerializeField] private DoorOpeningScripts _doorOpeningScripts = DoorOpeningScripts.V1;

	private HingeJoint doorHinge;

	public enum DoorOpeningScripts
	{
		V1,
		V2,
	}

	void Update()
	{
		switch (_doorOpeningScripts)
		{
			case DoorOpeningScripts.V1:
				DoorDragerV1();
				break;
			case DoorOpeningScripts.V2:
				DoorDragerV2(); 
				break;
		}


		//DoorDrager();
	}

	private void DoorDragerV1()
	{
		if (Physics.Raycast(_cam.transform.position, _cam.transform.forward, out RaycastHit hit, _doorDragingDistance, doorLayer))
		{
			if (Input.GetMouseButtonDown(0))
			{
				selectedDoor = hit.collider.gameObject.transform;
			}
		}

		if (selectedDoor != null)
		{
			HingeJoint joint = selectedDoor.GetComponent<HingeJoint>();
			JointMotor motor = joint.motor;

			//Create drag point object for reference where players mouse is pointing
			if (dragPointGameobject == null)
			{
				dragPointGameobject = new GameObject("Ray door");
				dragPointGameobject.transform.parent = selectedDoor;
			}

			Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
			dragPointGameobject.transform.position = ray.GetPoint(Vector3.Distance(selectedDoor.position, transform.position));
			dragPointGameobject.transform.rotation = selectedDoor.rotation;


			float delta = Mathf.Pow(Vector3.Distance(dragPointGameobject.transform.position, selectedDoor.position), 3);

			//Deciding if it is left or right door
			if (selectedDoor.GetComponent<MeshRenderer>().localBounds.center.x > selectedDoor.localPosition.x)
			{
				leftDoor = 1;
			}
			else
			{
				leftDoor = -1;
			}

			//Applying velocity to door motor
			float speedMultiplier = 60000;
			if (Mathf.Abs(selectedDoor.parent.forward.z) > 0.5f)
			{
				if (dragPointGameobject.transform.position.x > selectedDoor.position.x)
				{
					motor.targetVelocity = delta * -speedMultiplier * Time.deltaTime * leftDoor;
				}
				else
				{
					motor.targetVelocity = delta * speedMultiplier * Time.deltaTime * leftDoor;
				}
			}
			else
			{
				if (dragPointGameobject.transform.position.z > selectedDoor.position.z)
				{
					motor.targetVelocity = delta * -speedMultiplier * Time.deltaTime * leftDoor;
				}
				else
				{
					motor.targetVelocity = delta * speedMultiplier * Time.deltaTime * leftDoor;
				}
			}
			joint.motor = motor;

			if (Input.GetMouseButtonUp(0))
			{
				selectedDoor = null;
				motor.targetVelocity = 0;
				joint.motor = motor;
				Destroy(dragPointGameobject);


			}
		}
	}


	private void DoorDragerV2()
	{
		if (Input.GetMouseButtonDown(0)) // You can change the mouse button as needed.
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, _doorDragingDistance, doorLayer))
			{
				// The mouse was clicked on a door, so remember the door's HingeJoint.
				doorHinge = hit.transform.GetComponent<HingeJoint>();
			}
		}
		else if (Input.GetMouseButton(0) && doorHinge != null)
		{
			// Continue rotating the remembered door while the mouse button is held down.
			RotateDoor(doorHinge);
		}
		else
		{
			// Release the remembered door when the mouse button is not held.
			doorHinge = null;
		}
	}

	private void RotateDoor(HingeJoint hinge)
	{
		// Calculate the door rotation based on the mouse movement.
		float rotationAmount = -Input.GetAxis("Mouse X") * _doorOpeningForce;

		// Apply the rotation to the HingeJoint motor.
		JointMotor motor = hinge.motor;
		motor.targetVelocity = rotationAmount;
		hinge.motor = motor;
	}
}