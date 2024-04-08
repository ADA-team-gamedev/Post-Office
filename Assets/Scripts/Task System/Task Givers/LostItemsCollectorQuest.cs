using Items;
using System.Collections.Generic;
using TaskSystem;
using UnityEngine;

public class LostItemsCollectorQuest : MonoBehaviour
{
    [SerializeField] private List<Item> _lostItems = new();

	[SerializeField] private TaskData _lostItemsTask;

	private void Start()
	{
		foreach (var item in _lostItems)
		{
			if (item.gameObject.activeInHierarchy)
				item.OnPickUpItem += TryCompleteTask;
		}

		if (_lostItems.Count > 0)
			TaskManager.Instance.TryAddNewTask(_lostItemsTask);
		else
			Debug.LogWarning("Can't add task, because you don't set lost items!");
	}

	private void TryCompleteTask(Item item)
	{
		_lostItems.Remove(item);

		item.OnPickUpItem -= TryCompleteTask;

		if (_lostItems.Count <= 0)
		{
			if (TaskManager.Instance.TryGetTask(_lostItemsTask.Task.ID, out Task task))
				task.Complete();
		}
	}
}
