using UnityEngine;
using Items.Icon;
using TaskSystem;
using Level.Doors;
using Audio;
using Player;
using UnityModification;

namespace Items
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(BoxCollider))]
	public class RemoteControl : Item, IUsable
	{
		[SerializeField] private LayerMask _garageDoorLayer;

		[SerializeField] private TaskData _findRemoteControlTask;

		protected override void Start()
		{
			base.Start();
		}

		protected override void InitializeItem()
		{
			base.InitializeItem();

			OnPickUpItem += Completetask;

			TaskManager.Instance.OnNewCurrentTaskSet += ChangeItemIconState;

			TaskManager.Instance.TryAddNewTask(_findRemoteControlTask);
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

		public void Use(Interactor interactor)
		{
			AudioManager.Instance.PlaySound("Use Remote Control", transform.position);
			
			Transform playerCameraTransform = interactor.PlayerCamera.transform;

			if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit hit, Mathf.Infinity, _garageDoorLayer))
			{
				EditorDebug.DrawRay(playerCameraTransform.position, playerCameraTransform.forward, Color.green, 2);

				if (hit.transform.parent && hit.transform.parent.TryGetComponent(out GarageDoor garageDoor)) //transform.parent.TryGetComponent() - because garage door script lying on object without collider
					garageDoor.InteractRemotely();			
			}
			else
			{
				EditorDebug.DrawRay(playerCameraTransform.position, playerCameraTransform.forward, Color.red, 2);
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			OnPickUpItem -= Completetask;
		}
	}
}
