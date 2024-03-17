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

		public bool IsPicked { get; private set; } = false;

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
				ActivateAutoIconStateChanging();

			if (ItemIcon.EnableOnStart)
				ItemIcon.ShowIcon();
			else
				ItemIcon.HideIcon();

			OnPickUpItem += OnItemPicked;
			OnDropItem += OnItemDroped;
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

		private void OnItemPicked(Item item)
		{
			IsPicked = true;
		}

		private void OnItemDroped(Item item)
		{
			IsPicked = false;
		}		

		#endregion
	}
}