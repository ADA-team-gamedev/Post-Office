using Items;
using System.Collections.Generic;
using TaskSystem;
using UnityEngine;

public class LostedItemsCollectorQuest : MonoBehaviour
{
    [SerializeField] private List<Item> _lostedItems = new();

	[SerializeField] private TaskData _lostedItemsTask;

	private void Start()
	{
		foreach (var item in _lostedItems)
		{
			item.OnPickUpItem += TryCompleteTask;
		}

		if (_lostedItems.Count > 0)
			TaskManager.Instance.TryAddNewTask(_lostedItemsTask);
		else
			Debug.LogWarning("Can't add task, because you don't set losted items!");
	}

	private void TryCompleteTask(Item item)
	{
		if (_lostedItems.Count > 0)
		{
			item.OnPickUpItem -= TryCompleteTask;

			_lostedItems.Remove(item);

			return;
		}

		if (TaskManager.Instance.TryGetTask(_lostedItemsTask.Task.ID, out Task task))
			task.Complete();
	}
}
