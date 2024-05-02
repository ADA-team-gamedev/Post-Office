using Enemy;
using Items;
using System;
using System.Collections.Generic;
using TaskSystem.NoteBook;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Player
{
	[RequireComponent(typeof(PlayerDeathController))]
	public class PlayerInventory : MonoBehaviour
	{
		public static PlayerInventory Instance { get; private set; }

		#region Pickup fields 

		[SerializeField] private LayerMask _itemLayer;

		[Header("Values")]
		[SerializeField] private float _pickupRange = 3f;
		[SerializeField] private float _dropUpForce = 2f;
		[SerializeField] private float _dropForce = 1f;

		[Header("Objects")]
		[SerializeField] private Transform _playerHand;
		[SerializeField] private Camera _playerCamera;

		private Transform _currentObjectTransform;
		private Rigidbody _currentObjectRigidbody;
		private Collider _currentObjectCollider;

		public event Action<Item> OnItemPicked;
		public event Action<Item> OnItemDroped;
		public event Action OnItemChanged;

		#endregion

		#region Inventory fields

		[Header("Animation")]
		[SerializeField] private bool _playChangeItemAnimation = true;

		[SerializeField] private Animator _itemChangeAnimator;

		[SerializeField] private string _itemChangeTriggerName = "ItemChange";

		public const byte InventorySlotsAmount = 4;

		private int _currentSlotIndex = -1;

		private List<Item> _inventory;

		#endregion

		private PlayerDeathController _playerDeathController;

		private Tablet _tablet;

		private PlayerInput _playerInput;

		[Inject]
		private void Construct(PlayerInput playerInput, Tablet tablet)
		{
			_playerInput = playerInput;

			_tablet = tablet;

			_playerInput.Player.PickUpItem.performed += OnPickUpItem;

			_playerInput.Player.DropItem.performed += OnDropItem;

			_playerInput.Player.UseItem.performed += OnUseItem;

			_playerInput.Player.ScrollWheelY.performed += OnScrollWheelYChanged;

			_playerInput.Player.Hotbar1.performed += OnHotBar1Clicked;

			_playerInput.Player.Hotbar2.performed += OnHotBar2Clicked;

			_playerInput.Player.Hotbar3.performed += OnHotBar3Clicked;

			_playerInput.Player.Hotbar4.performed += OnHotBar4Clicked;
		}

		private void Awake()
		{
			if (Instance == null)
				Instance = this;
		}

		private void Start()
		{
			_playerDeathController = GetComponent<PlayerDeathController>();

			_playerDeathController.OnDied += ClearInventory;

			_inventory = new(InventorySlotsAmount);		
		}

		#region Pickup system

		private void TryPickupObject()
		{
			if (_inventory.Count >= InventorySlotsAmount)
				return;

			if (TryGetCurrentItem(out Box box) && box.TryGetComponent(out BoxEnemy boxEnemy) && !boxEnemy.IsPicked) //for box animation
				return;

			if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out RaycastHit hit, _pickupRange, _itemLayer))
			{
				if (hit.collider.TryGetComponent(out Item item) && item.CanBePicked)
				{
					_currentObjectTransform = hit.transform;
					_currentObjectRigidbody = hit.rigidbody;
					_currentObjectCollider = hit.collider;

					SetPickedItem();

					item.OnPickUpItem?.Invoke(item);

					AddItem(item);

					OnItemPicked?.Invoke(item);
				}
			}
		}

		public void DropItem()
		{
			if (!_currentObjectTransform)
				return;

			_currentObjectTransform.gameObject.SetActive(true);

			if (_currentObjectTransform.TryGetComponent(out Item item))
			{
				item.OnDropItem?.Invoke(item);

				OnItemDroped?.Invoke(item);
			}

			_currentObjectRigidbody.isKinematic = false;
			_currentObjectRigidbody.useGravity = true;

			_currentObjectTransform.SetParent(null);

			_currentObjectRigidbody.AddForce(_playerCamera.transform.forward * _dropForce, ForceMode.Impulse);
			_currentObjectRigidbody.AddForce(_playerCamera.transform.up * _dropUpForce, ForceMode.Impulse);

			_currentObjectTransform = null;
			_currentObjectRigidbody = null;

			_currentObjectCollider.enabled = true;
			_currentObjectCollider = null;

			RemoveItem();
		}

		private void SetPickedItem()
		{
			_currentObjectTransform.SetParent(_playerHand);

			_currentObjectRigidbody.isKinematic = true;
			_currentObjectRigidbody.useGravity = false;

			_currentObjectCollider.enabled = false;

			_currentObjectTransform.position = _playerHand.position;
			_currentObjectTransform.rotation = _playerHand.rotation;
		}

		#endregion

		#region Inventory system

		public bool IsContainsItem<T>(T item) where T : Item
		{
			foreach (var inventoryItem in _inventory)
			{
				if (Equals(inventoryItem, item))
					return true;		
			}

			return false;
		}

		public bool TryGetItem<T>(out T item) where T : Item
		{
			foreach (var inventoryItem in _inventory)
			{
				if (inventoryItem.TryGetComponent(out item))
					return true;		
			}

			item = default;

			return false;
		}

		public bool TryGetCurrentItem<T>(out T item) where T : Item
		{
			if (_currentSlotIndex < 0 || _currentSlotIndex >= _inventory.Count)
			{
				item = default;

				return false;
			}

			var itemInInventory = _inventory[_currentSlotIndex];

			if (itemInInventory != null)
			{
				if (itemInInventory.TryGetComponent(out item))
					return true;
			}

			item = default;

			return false;
		}

		public bool TryRemoveItem<T>(T item) where T : Item
		{
			if (!IsContainsItem(item))
				return false;

			_inventory.Remove(item);

			if (_currentSlotIndex == 0)
				_currentSlotIndex = _inventory.Count - 1;
			else if (_currentSlotIndex > 0)
				_currentSlotIndex--;

			ChangeSelectedSlot();

			return true;
		}

		private void HotbarSlotChange(int keyCodeNumber)
		{
			keyCodeNumber--;

			if (_inventory.Count <= keyCodeNumber)
				return;

			if (TryGetCurrentItem(out Box box) && box.TryGetComponent(out BoxEnemy boxEnemy) && !boxEnemy.IsPicked)
				return;

			_currentSlotIndex = keyCodeNumber;

			ChangeSelectedSlot();
		}

		private void AddItem(Item item)
		{
			_inventory.Add(item);

			_currentSlotIndex = _inventory.Count - 1;

			//if (_inventory.Count != 1 && !_changeItemWhenPickup)
			//	_currentSlotIndex--;

			ChangeSelectedSlot();
		}

		private void RemoveItem()
		{
			_inventory.RemoveAt(_currentSlotIndex);

			if (_currentSlotIndex == 0)
				_currentSlotIndex = _inventory.Count - 1;
			else if (_currentSlotIndex > 0)
				_currentSlotIndex--;

			ChangeSelectedSlot();
		}

		private void ChangeSelectedSlot()
		{
			if (_inventory.Count <= 0)
				return;

			for (int i = 0; i < _inventory.Count; i++)
			{
				if (i != _currentSlotIndex)
					_inventory[i].gameObject.SetActive(false);
			}

			var currentItem = _inventory[_currentSlotIndex];

			if (_playChangeItemAnimation)
				_itemChangeAnimator.SetTrigger(_itemChangeTriggerName);

			if (currentItem)
				currentItem.gameObject.SetActive(true);

			_currentObjectTransform = currentItem.transform;

			currentItem.TryGetComponent(out _currentObjectRigidbody);

			currentItem.TryGetComponent(out _currentObjectCollider);

			OnItemChanged?.Invoke();
		}

		#endregion

		#region Input entry points

		private void OnPickUpItem(InputAction.CallbackContext context)
		{
			TryPickupObject();
		}

		private void OnHotBar1Clicked(InputAction.CallbackContext context)
		{
			HotbarSlotChange(1);
		}

		private void OnHotBar2Clicked(InputAction.CallbackContext context)
		{
			HotbarSlotChange(2);
		}

		private void OnHotBar3Clicked(InputAction.CallbackContext context)
		{
			HotbarSlotChange(3);
		}

		private void OnHotBar4Clicked(InputAction.CallbackContext context)
		{
			HotbarSlotChange(4);
		}

		private void OnDropItem(InputAction.CallbackContext context)
		{
			if (!context.performed)
				return;

			DropItem();
		}

		private void OnUseItem(InputAction.CallbackContext context)
		{
			if (_currentSlotIndex < 0)
				return;

			var item = _inventory[_currentSlotIndex];

			if (item == null)
				return;

			if (item.TryGetComponent(out IUsable usableItem))
				usableItem.Use();
		}

		#region Inventory

		private void OnScrollWheelYChanged(InputAction.CallbackContext context)
		{
			if (_inventory.Count <= 0 || _tablet.IsViewing)
				return;
			
			float scrollWheelValue = context.ReadValue<float>();

			if (TryGetCurrentItem(out Box box) && box.TryGetComponent(out BoxEnemy boxEnemy) && !boxEnemy.IsPicked)
				return;

			if (scrollWheelValue != 0)
			{
				if (scrollWheelValue > 0)
					_currentSlotIndex++;
				else
					_currentSlotIndex--;

				if (_currentSlotIndex < 0)
					_currentSlotIndex = _inventory.Count - 1;
				else if (_currentSlotIndex > _inventory.Count - 1)
					_currentSlotIndex = 0;

				ChangeSelectedSlot();
			}
		}

		#endregion

		#endregion

		private void ClearInventory()
		{
			while (_inventory.Count > 0)
			{
				DropItem();
			}

			Destroy(this);
		}

		private void OnDestroy()
		{
			if (_playerInput != null)
			{
				_playerInput.Player.PickUpItem.performed -= OnPickUpItem;

				_playerInput.Player.DropItem.performed -= OnDropItem;

				_playerInput.Player.UseItem.performed -= OnUseItem;

				_playerInput.Player.ScrollWheelY.performed -= OnScrollWheelYChanged;

				_playerInput.Player.Hotbar1.performed -= OnHotBar1Clicked;

				_playerInput.Player.Hotbar2.performed -= OnHotBar2Clicked;

				_playerInput.Player.Hotbar3.performed -= OnHotBar3Clicked;

				_playerInput.Player.Hotbar4.performed -= OnHotBar4Clicked;
			}
		}
	}
}
