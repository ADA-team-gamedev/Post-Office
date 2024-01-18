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
	[SerializeField] private Transform _playerCamera;

	private Transform _currentObjectTransform;
	private Rigidbody _currentObjectRigidbody;
	private Collider _currentObjectCollider;

	#endregion

	#region Inventory fields

	private const byte _inventorySlotsAmount = 3;

	private int _currentSlotIndex = -1;

	private List<GameObject> Inventory;

	#endregion

	private PlayerInput _playerInput;

	private void Awake()
	{
		_playerInput = new();

		if (Instance == null)
			Instance = this;
	}

	private void Start()
	{
		Inventory = new(_inventorySlotsAmount);

		_playerInput.Player.Interact.performed += OnPickUpItem;

		_playerInput.Player.DropItem.performed += OnDropItem;

		_playerInput.Player.ScrollWheelY.performed += OnScrollWheelYChanged;

		_playerInput.Player.Hotbar1.performed += Hotbar1;
		_playerInput.Player.Hotbar2.performed += Hotbar2;
		_playerInput.Player.Hotbar3.performed += Hotbar3;
	}

	#region Pickup system
	public void TryPickupObject()
	{
		if (Inventory.Count < _inventorySlotsAmount)
		{
			if (Physics.Raycast(_playerCamera.position, _playerCamera.transform.forward, out RaycastHit hit, _pickupRange) && hit.collider.TryGetComponent(out IPickable pickable))
			{
				_currentObjectTransform = hit.transform;
				_currentObjectRigidbody = hit.rigidbody;
				_currentObjectCollider = hit.collider;

				SetPickedItem();

				AddItem(_currentObjectTransform.gameObject);

				pickable.OnPickUpItem?.Invoke();
			}
		}
	}

	public void DropItem()
	{
		if (!_currentObjectTransform)
			return;

		_currentObjectTransform.gameObject.SetActive(true);

		if (_currentObjectTransform.TryGetComponent(out IPickable pickable))
			pickable.OnDropItem?.Invoke();

		_currentObjectRigidbody.isKinematic = false;
		_currentObjectRigidbody.useGravity = true;

		_currentObjectTransform.SetParent(null);

		_currentObjectRigidbody.AddForce(_playerCamera.forward * _dropForce, ForceMode.Impulse);
		_currentObjectRigidbody.AddForce(_playerCamera.up * _dropUpForce, ForceMode.Impulse);

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

	public bool TryGetCurrentItem(out GameObject item)
	{
		if (_currentSlotIndex >= 0 && _currentSlotIndex < _inventorySlotsAmount)
			item = Inventory[_currentSlotIndex];
		else
			item = null;

		if (item)
			return true;

		return false;
	}

	private void HotbarSlotChange(int keyCodeNumber)
	{
		keyCodeNumber--;

		if (Inventory.Count <= keyCodeNumber)
			return;

		_currentSlotIndex = keyCodeNumber;

		ChangeSelectedSlot();
	}

	private void AddItem(GameObject item)
	{
		Inventory.Add(item);

		_currentSlotIndex++;

		Inventory[_currentSlotIndex].SetActive(false);

		if (Inventory.Count != 1 && !_changeItemWhenPickup)
			_currentSlotIndex--;

		ChangeSelectedSlot();
	}

	private void RemoveItem()
	{
		Inventory.RemoveAt(_currentSlotIndex);

		if (_currentSlotIndex == 0)
			_currentSlotIndex = Inventory.Count - 1;
		else if (_currentSlotIndex > 0)
			_currentSlotIndex--;

		ChangeSelectedSlot();
	}

	private void ChangeSelectedSlot()
	{
		if (Inventory.Count <= 0)
			return;

		foreach (GameObject item in Inventory)
		{
			if (item)
				item.SetActive(false);
		}

		if (Inventory[_currentSlotIndex])
			Inventory[_currentSlotIndex].SetActive(true);

		_currentObjectTransform = Inventory[_currentSlotIndex].transform;

		Inventory[_currentSlotIndex].TryGetComponent(out _currentObjectRigidbody);

		Inventory[_currentSlotIndex].TryGetComponent(out _currentObjectCollider);
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


	private void OnScrollWheelYChanged(InputAction.CallbackContext context)
	{
		if (Inventory.Count <= 0)
			return;

		float scrollWheelValue = context.ReadValue<float>();

		if (scrollWheelValue != 0)
		{
			if (scrollWheelValue > 0)
				_currentSlotIndex++;
			else
				_currentSlotIndex--;

			if (_currentSlotIndex < 0)
				_currentSlotIndex = Inventory.Count - 1;
			else if (_currentSlotIndex > Inventory.Count - 1)
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

	private void OnEnable()
	{
		_playerInput.Enable();
	}

	private void OnDisable()
	{
		_playerInput.Disable();
	}
}
