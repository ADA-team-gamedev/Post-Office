using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

	#region Pickup fields 

	[SerializeField] private LayerMask _pickupLayer;

    [field: SerializeField] public KeyCode PickupKey { get; set; } = KeyCode.E;
    [field: SerializeField] public KeyCode DropKey { get; set; } = KeyCode.G;

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

	[SerializeField][Range(1, 10)] private int _inventorySlotsAmount = 3;	

	private int _currentSlotIndex = -1;

	private List<GameObject> Inventory;

	#endregion

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
	}

	void Start()
    {
		Inventory = new(_inventorySlotsAmount);     
	}

    private void Update()
    {
		TryPickupObject();
		
		HandleInventoryInput();


	}

	#region Pickup system
	public void TryPickupObject() //make interactable
    {
		if (Input.GetKeyDown(PickupKey) && Inventory.Count < _inventorySlotsAmount)
		{
			if (Physics.Raycast(_playerCamera.position, _playerCamera.transform.forward, out RaycastHit hit, _pickupRange) && hit.collider.TryGetComponent(out IPickable pickable))
			{
				_currentObjectTransform = hit.transform;
				_currentObjectRigidbody = hit.rigidbody;
				_currentObjectCollider = hit.collider;

				pickable.PickUpItem();

				SetPickedItem();
			}	
		}

		if (_currentObjectTransform) //IsEquipped
		{
			if (Input.GetKeyDown(DropKey))
				DropItem();		
		}

		////interactable version >>>

		//if (Inventory.Count >= _inventorySlotsAmount)
		//	return;

		//if (Physics.Raycast(_playerCamera.position, _playerCamera.transform.forward, out RaycastHit hit, _pickupRange) && hit.collider.TryGetComponent(out IPickable pickable))
		//{
		//	_currentObjectTransform = hit.transform;
		//	_currentObjectRigidbody = hit.rigidbody;
		//	_currentObjectCollider = hit.collider;

		//	pickable.PickUpItem();

		//	SetPickedItem();
		//}
	}

	public void DropItem()
	{
		if (!_currentObjectTransform)
			return;	

		_currentObjectTransform.gameObject.SetActive(true);

		if (_currentObjectTransform.TryGetComponent(out IPickable pickable))
			pickable.DropItem();

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

		AddItem(_currentObjectTransform.gameObject);
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

	private void HandleInventoryInput()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1) && Inventory.Count > 0)
		{
			_currentSlotIndex = 0;

			ChangeSelectedSlot();
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2) && Inventory.Count > 1)
		{
			_currentSlotIndex = 1;

			ChangeSelectedSlot();
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3) && Inventory.Count > 2)
		{
			_currentSlotIndex = 2;

			ChangeSelectedSlot();
		}

		if (Inventory.Count > 0)
		{
			float scroll = -Input.GetAxis("Mouse ScrollWheel");

			if (scroll != 0)
			{
				if (scroll > 0)
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
	}

	private void AddItem(GameObject item)
	{
		Inventory.Add(item);

		_currentSlotIndex++;

		Inventory[_currentSlotIndex].SetActive(false);

		if (Inventory.Count == 1 || _changeItemWhenPickup)
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
}
