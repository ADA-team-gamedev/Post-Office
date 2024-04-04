using Enemy;
using InputSystem;
using Items;
using System;
using System.Collections.Generic;
using TaskSystem.NoteBook;
using UnityEngine;
using UnityEngine.InputSystem;

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

		private const byte _inventorySlotsAmount = 4;

		private int _currentSlotIndex = -1;

		private List<GameObject> _inventory;

		#endregion

		private PlayerDeathController _playerDeathController;

		[SerializeField] private Tablet _noteBook;

		private void Awake()
		{
			if (Instance == null)
				Instance = this;
		}

		private void Start()
		{
			_playerDeathController = GetComponent<PlayerDeathController>();

			_playerDeathController.OnDied += ClearInventory;

			_inventory = new(_inventorySlotsAmount);

			InputManager.Instance.PlayerInput.Player.PickUpItem.performed += OnPickUpItem;

			InputManager.Instance.PlayerInput.Player.DropItem.performed += OnDropItem;

			InputManager.Instance.PlayerInput.Player.UseItem.performed += OnUseItem;

			InputManager.Instance.PlayerInput.Player.ScrollWheelY.performed += OnScrollWheelYChanged;

			InputManager.Instance.PlayerInput.Player.Hotbar1.performed += (context) =>
			{
				HotbarSlotChange(1);
			};

			InputManager.Instance.PlayerInput.Player.Hotbar2.performed += (context) =>
			{
				HotbarSlotChange(2);
			};

			InputManager.Instance.PlayerInput.Player.Hotbar3.performed += (context) =>
			{
				HotbarSlotChange(3);
			};

			InputManager.Instance.PlayerInput.Player.Hotbar4.performed += (context) =>
			{
				HotbarSlotChange(4);
			};
		}

		#region Pickup system

		private void TryPickupObject()
		{
			if (_inventory.Count >= _inventorySlotsAmount)
				return;

			if (TryGetCurrentItem(out Box box) && box.TryGetComponent(out BoxEnemy boxEnemy) && !boxEnemy.IsPicked)
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

					AddItem(_currentObjectTransform.gameObject);

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
			if (!TryGetItem(out item))
				return false;

			_inventory.Remove(item.gameObject);

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

		private void AddItem(GameObject item)
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
					_inventory[i].SetActive(false);
			}

			GameObject currentItem = _inventory[_currentSlotIndex];

			if (currentItem)
				currentItem.SetActive(true);

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

			GameObject item = _inventory[_currentSlotIndex];

			if (item == null)
				return;

			if (item.TryGetComponent(out IUsable usableItem))
				usableItem.Use();
		}

		#region Inventory

		private void OnScrollWheelYChanged(InputAction.CallbackContext context)
		{
			if (_inventory.Count <= 0 || _noteBook.IsViewing)
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
	}
}
