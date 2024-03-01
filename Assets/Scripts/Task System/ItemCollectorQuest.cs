using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ItemCollectorQuest : MonoBehaviour
{
	[Header("Task")]
	[SerializeField] private string _playerTag = "Player";

	[SerializeField] private bool _giveTaskOnStart = false;

	[SerializeField] private TaskData _addedTask;

	[Header("Items")]
	[SerializeField] private List<Item> _neededItems;
	
	private List<Item> _addedItem = new();

	private bool _isTaskAdded = false;

	private void Start()
	{
		GetComponent<BoxCollider>().isTrigger = true;

		if (_giveTaskOnStart)
			GiveTaskToPlayer();	
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!_isTaskAdded && other.CompareTag(_playerTag))
			GiveTaskToPlayer();	

		if (other.TryGetComponent(out Item item) && !_addedItem.Contains(item))
		{
			_addedItem.Add(item);

			item.OnPickUpItem += RemoveBox;

			TryCompleteTask();		
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.TryGetComponent(out Item item))
		{
			if (_addedItem.Contains(item))
				_addedItem.Remove(item);

			TryCompleteTask();
		}
	}

	private void GiveTaskToPlayer()
	{
		if (_isTaskAdded)
			return;

		TaskManager.Instance.SetNewCurrentTask(_addedTask);

		_isTaskAdded = true;
	}

	private void TryCompleteTask()
	{
		if (IsAllBoxesCollected())
			TaskManager.Instance.CurrentTask.Complete();
	}

	private bool IsAllBoxesCollected()
	{
		if (_neededItems.Count != _addedItem.Count)
			return false;

		for (int i = 0; i < _neededItems.Count; i++)
		{
			if (!_neededItems.Contains(_addedItem[i]))
				return false;
		}

		return true;
	}

	private void RemoveBox()
	{
		if (PlayerInventory.Instance.TryGetCurrentItem(out Item item))	
			_addedItem.Remove(item);

		TryCompleteTask();
	}
}
