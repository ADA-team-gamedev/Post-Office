using System.Collections.Generic;
using UnityEngine;

public class ItemCollectorQuest : MonoBehaviour
{
	[Header("Task")]
	[SerializeField] private string _playerTag = "Player";

	[SerializeField] private bool _giveTaskOnStart = false;

	[SerializeField] private TaskData _addedTask;

	[Header("Items")]
	[SerializeField] private List<Box> _neededItems;
	
	private List<Box> _addedBoxes = new();

	private bool _isTaskAdded = false;

	private void Start()
	{
		if (_giveTaskOnStart)
			GiveTaskToPlayer();	
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag(_playerTag))
			GiveTaskToPlayer();	

		if (other.TryGetComponent(out Box box) && !_addedBoxes.Contains(box))
		{
			_addedBoxes.Add(box);

			box.OnPickUpItem += RemoveBox;

			TryCompleteTask();		
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.TryGetComponent(out Box box))
		{
			if (_addedBoxes.Contains(box))
				_addedBoxes.Remove(box);

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
		if (_neededItems.Count != _addedBoxes.Count)
			return false;

		for (int i = 0; i < _neededItems.Count; i++)
		{
			if (!_neededItems.Contains(_addedBoxes[i]))
				return false;
		}

		return true;
	}

	private void RemoveBox()
	{
		if (PlayerInventory.Instance.TryGetCurrentItem(out Box box))	
			_addedBoxes.Remove(box);

		TryCompleteTask();
	}
}
