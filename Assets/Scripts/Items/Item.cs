using System;
using UnityEngine;
using Items.Icon;

namespace Items
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Collider))]
	public class Item : MonoBehaviour
	{
		[field: Header("Item")]

		[field: SerializeField] public bool CanBePicked { get; set; } = true;

		[field: SerializeField] public ItemIcon ItemIcon { get; private set; }

		public Action<Item> OnPickUpItem { get; set; }

		public Action<Item> OnDropItem { get; set; }

		private void Start()
		{
			InitializeItem();
		}

		private void Update()
		{
			ItemIcon.RotateIconToObject();
		}

		protected virtual void InitializeItem()
		{
			InitializeItemIcon();
		}

		#region Icon Logic

		private void InitializeItemIcon()
		{
			if (ItemIcon.ChangeIconStateAutomatically)
			{
				ActivateAutoIconStateChanging();

				ItemIcon.ShowIcon();
			}
			else
			{
				ItemIcon.HideIcon();
			}
		}

		public void ActivateAutoIconStateChanging()
		{
			OnPickUpItem += ItemIcon.HideIcon;

			OnDropItem += ItemIcon.ShowIcon;
		}

		public void DeactivateAutoIconStateChanging()
		{
			OnPickUpItem -= ItemIcon.HideIcon;

			OnDropItem -= ItemIcon.ShowIcon;
		}

		#endregion
	}
}