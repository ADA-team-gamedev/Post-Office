using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]

public class RemoteControl : Item, IUsable
{
	[Header("Remote control")]
	[SerializeField] private Camera _playerCamera;

	[SerializeField] private TaskData _findRemoteControlTask;

	private void Start()
	{
		Initialize();
	}

	protected override void Initialize()
	{
		base.Initialize();

		OnPickUpItem += Completetask;

		TaskManager.Instance.TryAddNewTask(_findRemoteControlTask);

		TaskManager.Instance.OnNewCurrentTaskSet += ChangeItemIconState;
	}

	private void Update()
	{
		base.RotateIconToObject(_playerCamera.transform.position);
	}

	private void ChangeItemIconState(Task currentTask)
	{
		if (currentTask.ID != _findRemoteControlTask.Task.ID)
		{
			base.HideIcon();

			return;
		}	

		base.ShowIcon();
	}

	private void Completetask(Item item)
	{
		if (!TaskManager.Instance.TryGetTask(_findRemoteControlTask.Task.ID, out Task task))
			return;

		base.HideIcon();

		TaskManager.Instance.OnNewCurrentTaskSet -= ChangeItemIconState;

		task.Complete();

		OnPickUpItem -= Completetask;

		base.ActivateAutoIconStateChanging();
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
