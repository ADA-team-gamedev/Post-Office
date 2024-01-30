using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]

public class RemoteControl : Item, IUsable
{
	[SerializeField] private Camera _playerCamera;

	[SerializeField] private TaskData _findRemoteControlTask;

	private void Start()
	{
		TaskManager.Instance.AddNewTask(_findRemoteControlTask);

		OnPickUpItem += Completetask;
	}

	private void Completetask()
	{
		if (!TaskManager.Instance.TryGetTaskByType(_findRemoteControlTask.Task.ID, out Task task))
			return;

		task.Complete();
	}

	public void Use()
	{
		if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out RaycastHit hit))
		{
			if (hit.transform.parent && hit.transform.parent.TryGetComponent(out GarageDoor garageDoor)) //transform.parent.TryGetComponent() - because garage door script lying on object without collider
				garageDoor.InteractRemotely();
		}
	}
}
