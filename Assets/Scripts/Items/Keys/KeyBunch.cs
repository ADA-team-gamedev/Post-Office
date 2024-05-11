using Audio;
using Player;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Items.Keys
{
	public class KeyBunch : Item
	{
		[SerializeField] private Transform _pickedKeysParent;

		private List<DoorKeyType> _keyTypes = new();

		[SerializeField] private List<Key> _keysOnStart;

		[SerializeField] private BunchKey[] _keyModels;

		private MaterialPropertyBlock _propertyBlock;

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

			OnDropItem += OnPlayerDropBunch;
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
			PlayerInventory.Instance.OnItemPicked += OnPlayerPickUpItem;

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

		private void OnPlayerDropBunch(Item item)
		{
			PlayerInventory.Instance.OnItemPicked -= OnPlayerPickUpItem;
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

			OnDropItem -= OnPlayerDropBunch;
		}
	}

	[Serializable]
	public struct BunchKey
	{
		[field: SerializeField] public GameObject KeyObject { get; private set; }

		[field: SerializeField] public Renderer LabelRenderer { get; private set; }
	}
}