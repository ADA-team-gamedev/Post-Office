using Items;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Level.Spawners.LostItemSpawner
{
    public class LostItemSpawner : MonoBehaviour
    {
		[Header("Sticker")]

		[SerializeField] private Transform _stickersParent;

		[SerializeField] private LostItemSticker[] _stikerPrefabs;

		[SerializeField] private List<Transform> _lostItemStickersPosition;

		[Header("Lost Item")]

		[SerializeField] private bool _isConstantLostItemAmount = false;

		[SerializeField, Min(_minLostItemAmount)] private int _maxItemAmount;

		[SerializeField] private Transform _lostItemsParent;

		[SerializeField] private List<LostItem> _lostItemPrefabs;

		[SerializeField] private List<Transform> _lostItemSpawnPoints;

		public event Action<Dictionary<Item, LostItemSticker>> OnLostItemSpawned;

		private const int _minLostItemAmount = 3;

		private void Start()
		{
			if (TrySpawnLostItems(out Dictionary<Item, LostItemSticker> lostItems))
			{
				OnLostItemSpawned?.Invoke(lostItems);
			}
			else
			{
#if UNITY_EDITOR
				Debug.LogWarning("Can't add task, because you don't set lost items!");
#endif
			}
		}

		private bool TrySpawnLostItems(out Dictionary<Item, LostItemSticker> lostItems)
		{
			lostItems = new();

			bool noObjectInCollections = _lostItemStickersPosition.Count <= 0 || _lostItemSpawnPoints.Count <= 0 || _lostItemPrefabs.Count <= 0 || _stikerPrefabs.Length <= 0;

			bool wrongLostItemAmount = _maxItemAmount <= 0 || _lostItemPrefabs.Count < _maxItemAmount || _lostItemStickersPosition.Count < _maxItemAmount;

			if (noObjectInCollections || wrongLostItemAmount)
				return false;

			int lostItemCount;

			if (_isConstantLostItemAmount)
				lostItemCount = _maxItemAmount;
			else
				lostItemCount = Random.Range(_minLostItemAmount, _maxItemAmount + 1); // +1 for exclusive number

			for (int i = 0; i < lostItemCount && _lostItemPrefabs.Count > 0 && _lostItemSpawnPoints.Count > 0 && _lostItemStickersPosition.Count > 0; i++)
			{
				LostItem spawnedLostItem = Instantiate(ChooseLostItem(), ChooseLostItemPosition(), Quaternion.identity, _lostItemsParent);

				Transform stikerPositionOnDesk = ChooseLostItemStickerPosition();

				LostItemSticker lostItemSticker = Instantiate(ChooseStickerPrefab(), stikerPositionOnDesk.position, stikerPositionOnDesk.rotation, _stickersParent);

				SetStickerTexture(lostItemSticker.Photo, spawnedLostItem.StikckerTexture);

				lostItems.Add(spawnedLostItem, lostItemSticker);
			}

			return true;
		}

		private LostItem ChooseLostItem()
		{
			int lostItemPositionIndex = Random.Range(0, _lostItemPrefabs.Count);

			LostItem item = _lostItemPrefabs[lostItemPositionIndex];

			_lostItemPrefabs.RemoveAt(lostItemPositionIndex);

			return item;
		}

		private Vector3 ChooseLostItemPosition()
		{
			int lostItemPositionIndex = Random.Range(0, _lostItemSpawnPoints.Count);

			Vector3 lostItemPosition = _lostItemSpawnPoints[lostItemPositionIndex].position;

			_lostItemSpawnPoints.RemoveAt(lostItemPositionIndex);

			return lostItemPosition;
		}

		private Transform ChooseLostItemStickerPosition()
		{
			int stikerPositionIndex = Random.Range(0, _lostItemStickersPosition.Count);

			Transform stikerPositionOnDesk = _lostItemStickersPosition[stikerPositionIndex];

			_lostItemStickersPosition.RemoveAt(stikerPositionIndex);

			return stikerPositionOnDesk;
		}

		private LostItemSticker ChooseStickerPrefab()
		{
			return _stikerPrefabs[Random.Range(0, _stikerPrefabs.Length)];
		}

		private void SetStickerTexture(Transform stikerPhoto, Texture stickerPhotoTexture)
		{
			Renderer stikerRenderer = stikerPhoto.GetComponent<Renderer>();
			
			stikerRenderer.material.mainTexture = stickerPhotoTexture;
		}

		private void OnValidate()
		{
			if (_maxItemAmount > _lostItemStickersPosition.Count)
				_maxItemAmount = _lostItemStickersPosition.Count;
		}
	}
}