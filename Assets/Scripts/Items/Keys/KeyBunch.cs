using Audio;
using Player.Inventory;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Items.Keys
{
	public class KeyBunch : Item
	{
		#region KeyBunch

		[SerializeField] private Transform _pickedKeysParent;

		private List<DoorKeyType> _keyTypes = new();

		[SerializeField] private List<Key> _keysOnStart;

		[SerializeField] private BunchKey[] _keyModels;

		private MaterialPropertyBlock _propertyBlock;

		#endregion

		protected override void Start()
		{
			base.Start();

			_propertyBlock = new();

			foreach (var key in _keyModels)
			{
				key.KeyObject.SetActive(false);
			}

			FillBunchOnStart();

			OnPickUpItem += OnPlayerPickupBunch;
		}

		public void AddKey(Key key)
		{
			if (!TryAddKey(key))
			{
#if UNITY_EDITOR
				Debug.LogWarning("Can't add key!");
#endif
				return;
			}

			Destroy(key.gameObject);

			AudioManager.Instance.PlaySound("Pickup Key", transform.position);
		}

		private bool TryAddKey(Key key)
		{
			if (_keyModels.Length <= 0 || _keyTypes.Count >= _keyModels.Length)
				return false;

			if (IsContainsKey(key.KeyType))
			{
#if UNITY_EDITOR
				Debug.LogWarning($"Key {key.KeyType} is already contains in key bunch!");
#endif
				return false;
			}

			_keyTypes.Add(key.KeyType);

			int keyIndex = Mathf.Clamp(_keyTypes.Count - 1, 0, _keyModels.Length - 1);

			BunchKey bunchKey = _keyModels[keyIndex];

			PaintKeyLabel(bunchKey, key);

			bunchKey.KeyObject.SetActive(true);

			return true;
		}

		private void OnPlayerPickupBunch(Item item)
		{
			Interactor.Inventory.OnItemPicked += OnPlayerPickUpRandomItem;

			bool addedKey = false;

			for (int i = 0; i < PlayerInventory.InventorySlotsAmount; i++)
			{
				if (!Interactor.Inventory.TryGetItem(out Key key))
					break;		

				if (TryAddKey(key) && Interactor.Inventory.TryRemoveItem(key))
				{
					addedKey = true;

					Destroy(key.gameObject);
				}
			}

			if (addedKey)
				AudioManager.Instance.PlaySound("Pickup Key", transform.position);
		}

		protected override void OnItemDroped(Item item)
		{
			Interactor.Inventory.OnItemPicked -= OnPlayerPickUpRandomItem;

			base.OnItemDroped(item);
		}

		private void OnPlayerPickUpRandomItem(Item item)
		{
			if (!item.TryGetComponent(out Key key) || IsContainsKey(key.KeyType))
				return;

			if (!TryAddKey(key))
				return;

			if (Interactor.Inventory.TryRemoveItem(key))
			{
				Destroy(key.gameObject);

				AudioManager.Instance.PlaySound("Pickup Key", transform.position);
			}
		}

		public bool IsContainsKey(DoorKeyType keyType)
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
			if (_keysOnStart.Count <= 0)
				return;

			while (_keysOnStart.Count > 0 && _keysOnStart.Count < _keyModels.Length)
			{
				var key = _keysOnStart[0];

				if (TryAddKey(key))
				{
					key.ItemIcon.HideIcon();

					_keysOnStart.Remove(key);

					Destroy(key.gameObject);
				}
			}
		}	

		private void PaintKeyLabel(BunchKey bunchKey, Key key)
		{
			_propertyBlock.SetColor(Key.LabelBaseColorName, key.LabelColor);

			bunchKey.LabelRenderer.SetPropertyBlock(_propertyBlock);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			OnPickUpItem -= OnPlayerPickupBunch;
		}
	}

	[Serializable]
	public struct BunchKey
	{
		[field: SerializeField] public GameObject KeyObject { get; private set; }

		[field: SerializeField] public Renderer LabelRenderer { get; private set; }
	}
}