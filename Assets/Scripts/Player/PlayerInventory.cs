using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
	public static PlayerInventory Instance { get; private set; }

	#region Pickup fields 

	[Header("Values")]
	[SerializeField] private float _pickupRange = 3f;
	[SerializeField] private float _dropUpForce = 2f;
	[SerializeField] private float _dropForce = 1f;

	[SerializeField] private bool _changeItemWhenPickup = false;

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

	private const byte _inventorySlotsAmount = 3;

	private int _currentSlotIndex = -1;

	private List<GameObject> _inventory;

	#endregion

	private PlayerInput _playerInput;

	private PlayerDeathController _playerDeathController;

	private void Awake()
	{
		_playerInput = new();

		if (Instance == null)
			Instance = this;
	}

	private void Start()
	{
		_playerDeathController = GetComponent<PlayerDeathController>();

		_playerDeathController.OnDeath += ClearInventory;

		_inventory = new(_inventorySlotsAmount);
		
		_playerInput.Player.PickUpItem.performed += OnPickUpItem;

		_playerInput.Player.DropItem.performed += OnDropItem;

		_playerInput.Player.UseItem.performed += OnUseItem;

		_playerInput.Player.ScrollWheelY.performed += OnScrollWheelYChanged;

		_playerInput.Player.Hotbar1.performed += Hotbar1;
		_playerInput.Player.Hotbar2.performed += Hotbar2;
		_playerInput.Player.Hotbar3.performed += Hotbar3;
	}

	#region Pickup system

	public void TryPickupObject()
	{
		if (_inventory.Count < _inventorySlotsAmount)
		{
			if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out RaycastHit hit, _pickupRange) && hit.collider.TryGetComponent(out Item item))
			{
				_currentObjectTransform = hit.transform;
				_currentObjectRigidbody = hit.rigidbody;
				_currentObjectCollider = hit.collider;

				SetPickedItem();

				AddItem(_currentObjectTransform.gameObject);

				item.OnPickUpItem?.Invoke();

				OnItemPicked?.Invoke(item);
			}
		}
	}

	private void DropItem()
	{
		if (!_currentObjectTransform)
			return;

		_currentObjectTransform.gameObject.SetActive(true);

		if (_currentObjectTransform.TryGetComponent(out Item item))
		{
			item.OnDropItem?.Invoke();

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

	private void HotbarSlotChange(int keyCodeNumber)
	{
		keyCodeNumber--;

		if (_inventory.Count <= keyCodeNumber)
			return;

		_currentSlotIndex = keyCodeNumber;

		ChangeSelectedSlot();
	}

	private void AddItem(GameObject item)
	{
		_inventory.Add(item);

		_currentSlotIndex++;

		if (_inventory.Count != 1 && !_changeItemWhenPickup)
			_currentSlotIndex--;

		ChangeSelectedSlot();
	}

	public void RemoveItem()
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
		if (_inventory.Count <= 0 || _playerInput.UI.NoteBook.IsPressed())
			return;

		float scrollWheelValue = context.ReadValue<float>();

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

	private void Hotbar1(InputAction.CallbackContext context)
	{
		HotbarSlotChange(1);
	}

	private void Hotbar2(InputAction.CallbackContext context)
	{
		HotbarSlotChange(2);
	}

	private void Hotbar3(InputAction.CallbackContext context)
	{
		HotbarSlotChange(3);
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

	private void OnEnable()
	{
		_playerInput.Enable();
	}

	private void OnDisable()
	{
		_playerInput.Disable();
	}
}
