using Items;
using Level.Spawners.LostItemSpawner;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityModification;

namespace TaskSystem.TaskGivers
{
	public class LostItemsCollectorQuest : DestructiveBehaviour<LostItemsCollectorQuest>
	{
		[SerializeField] private TaskData _lostItemsTask;

		private Dictionary<Item, LostItemSticker> _lostItems;

		private LostItemSpawner _lostItemSpawner;

		[Inject]
		private void Construct(LostItemSpawner lostItemSpawner)
		{
			_lostItemSpawner = lostItemSpawner;

			_lostItemSpawner.OnLostItemSpawned += GiveLostItemQuest;

			_lostItemSpawner.OnObjectDestroyed += OnLostItemSpawnerDestroyed;
		}

		private void GiveLostItemQuest(Dictionary<Item, LostItemSticker> lostItems)
		{
			_lostItemSpawner.OnLostItemSpawned -= GiveLostItemQuest;

			if (lostItems.Count <= 0 || !TaskManager.Instance.TryAddNewTask(_lostItemsTask))
			{
				EditorDebug.LogWarning($"Can't add \"Find Lost Item\" task!");

				return;
			}

			_lostItems = lostItems;

			foreach (var item in _lostItems)
			{
				item.Key.OnPickUpItem += OnPlayerFindItem;
			}
		}

		private void OnPlayerFindItem(Item item)
		{
			if (!_lostItems.Remove(item, out LostItemSticker sticker))
				return;

			sticker.gameObject.SetActive(false);

			item.OnPickUpItem -= OnPlayerFindItem;

			if (_lostItems.Count <= 0)
				CompleteTask();
		}

		private void CompleteTask()
		{
			if (!TaskManager.Instance.TryGetTask(_lostItemsTask.Task.ID, out Task task))
				return;

			task.Complete();
		}

		private void OnLostItemSpawnerDestroyed(LostItemSpawner lostItemSpawner)
		{
			lostItemSpawner.OnObjectDestroyed -= OnLostItemSpawnerDestroyed;

			lostItemSpawner.OnLostItemSpawned -= GiveLostItemQuest;
		}
	}
}