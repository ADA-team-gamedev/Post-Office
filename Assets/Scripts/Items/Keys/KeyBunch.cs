using Audio;
using Player;
using System.Collections.Generic;
using UnityEngine;

namespace Items.Keys
{
	public class KeyBunch : Item
	{
		[SerializeField] private Transform _pickedKeysParent;

		private List<DoorKeyTypes> _keyTypes = new();

		[SerializeField] private Key[] _keysOnStart;

		[SerializeField] private GameObject[] _keyModels;

		private void Start()
		{
			InitializeItem();

			foreach (var key in _keyModels)
			{
				key.SetActive(false);
			}

			FillBunchOnStart();

			OnPickUpItem += OnPlayerPickupBunch;

			OnPickUpItem += (item) =>
			{
				PlayerInventory.Instance.OnItemPicked += OnPlayerPickUpItem;
			};

			OnDropItem += (item) =>
			{
				PlayerInventory.Instance.OnItemPicked -= OnPlayerPickUpItem;
			};		
		}

		public bool TryAddKey(Key key)
		{
			if (_keyModels.Length <= 0 || _keyTypes.Count >= _keyModels.Length)
				return false;

			if (IsContainsKey(key.KeyType))
			{
				Debug.LogWarning($"Key {key.KeyType} is already contains in key bunch!");

				return false;
			}

			_keyTypes.Add(key.KeyType);

			int keyIndex = Mathf.Clamp(_keyTypes.Count - 1, 0, _keyModels.Length - 1);

			_keyModels[keyIndex].SetActive(true);

			return true;
		}

		private void OnPlayerPickupBunch(Item item)
		{
			bool addedKey = false;

			for (int i = 0; i < PlayerInventory.InventorySlotsAmount; i++)
			{
				if (!PlayerInventory.Instance.TryGetItem(out Key key))
					break;		

				if (TryAddKey(key) && PlayerInventory.Instance.TryRemoveItem(key))
				{
					addedKey = true;

					Destroy(key.gameObject);
				}
			}

			if (addedKey)
				AudioManager.Instance.PlaySound("Pickup Key", transform.position);
		}

		private void OnPlayerPickUpItem(Item item)
		{
			if (!item.TryGetComponent(out Key key) || IsContainsKey(key.KeyType))
				return;

			if (!TryAddKey(key))
				return;

			if (PlayerInventory.Instance.TryRemoveItem(key))
			{
				Destroy(key.gameObject);

				AudioManager.Instance.PlaySound("Pickup Key", transform.position);
			}
		}

		public bool IsContainsKey(DoorKeyTypes keyType)
		{
			foreach (var key in _keyTypes)
			{
				if (key == keyType)
					return true;
			}

			return false;
		}

		private void FillBunchOnStart()
		{
			if (_keysOnStart.Length <= 0)
				return;

			foreach (var key in _keysOnStart)
			{
				if (TryAddKey(key))
				{
					key.ItemIcon.HideIcon();
					
					Destroy(key.gameObject);
				}
			}
		}
	}
}