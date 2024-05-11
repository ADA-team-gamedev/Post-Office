using System;
using UnityEngine;
using Items.Icon;
using Audio;
using UnityEngine.Modification;

namespace Items
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Collider))]
	public class Item : DestructiveBehaviour<Item>
	{
		[field: Header("Item")]

		[field: SerializeField] private bool _canBePicked = true;

		public bool CanBePicked
		{
			get { return _canBePicked; }
			set
			{
				_canBePicked = value;

				OnItemPickingPropertyChanged?.Invoke();
			}
		}

		public bool IsPicked { get; private set; } = false;

		[Header("Sounds")]

		[SerializeField] private string _dropSound = "Drop Item";
		[SerializeField] private string _pickupSound = "Pickup Item";

		[field: SerializeField] public ItemIcon ItemIcon { get; private set; }

		public Action<Item> OnPickUpItem { get; set; }

		public Action<Item> OnDropItem { get; set; }

		public Action OnItemPickingPropertyChanged {  get; set; }

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
				ItemIcon.ShowIcon(this);
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

			AudioManager.Instance.PlaySound(_pickupSound, transform.position);
		}

		private void OnItemDroped(Item item)
		{
			IsPicked = false;

			AudioManager.Instance.PlaySound(_dropSound, transform.position);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy(); 
			
			OnPickUpItem -= ItemIcon.HideIcon;

			OnDropItem -= ItemIcon.ShowIcon;

			ItemIcon.HideIcon();
		}

		#endregion
	}
}