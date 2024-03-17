using UnityEngine;
using Items.Icon;
using TaskSystem;
using Level.Doors;

namespace Items
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(BoxCollider))]
	public class RemoteControl : Item, IUsable
	{
		[Header("Remote control")]
		[SerializeField] private Camera _playerCamera;

		[SerializeField] private TaskData _findRemoteControlTask;

		private void Start()
		{
			InitializeItem();
		}

		protected override void InitializeItem()
		{
			base.InitializeItem();

			OnPickUpItem += Completetask;

			TaskManager.Instance.TryAddNewTask(_findRemoteControlTask);

			TaskManager.Instance.OnNewCurrentTaskSet += ChangeItemIconState;
		}

		private void Update()
		{
			ItemIcon.RotateIconToObject();
		}

		private void ChangeItemIconState(Task currentTask)
		{
			if (currentTask.ID != _findRemoteControlTask.Task.ID)
			{
				ItemIcon.HideIcon();

				return;
			}

			ItemIcon.ShowIcon(this);
		}

		private void Completetask(Item item)
		{
			if (!TaskManager.Instance.TryGetTask(_findRemoteControlTask.Task.ID, out Task task))
				return;

			ItemIcon.HideIcon();

			TaskManager.Instance.OnNewCurrentTaskSet -= ChangeItemIconState;

			task.Complete();

			OnPickUpItem -= Completetask;

			ActivateAutoIconStateChanging();
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
}
