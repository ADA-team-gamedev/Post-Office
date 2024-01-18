using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class SortQuest : MonoBehaviour
{
	[SerializeField] private TaskData _addedTask;

	[SerializeField] private List<Box> _neededBoxes;
	
	private List<Box> _addedBoxes = new();

	[SerializeField] private TextMeshProUGUI text;

	private void Start()
	{
		if (TaskManager.Instance.TryGetTaskByType(_addedTask.Task.Type, out TaskData neededTask))
			neededTask.Task.OnCompleted += FinishMovingTask;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && TaskManager.Instance.CurrentTask != _addedTask.Task)
		{
			TaskManager.Instance.TryAddNewTask(_addedTask);

			TaskManager.Instance.SetNewCurrentTask(_addedTask);
			
			TaskManager.Instance.CurrentTask.OnCompleted += FinishMovingTask;

			text.text = TaskManager.Instance.CurrentTask.Description;
		}

		if (other.TryGetComponent(out Box box) && !_addedBoxes.Contains(box))
		{
			_addedBoxes.Add(box);

			box.OnPickUpItem += RemoveBox;

			if (IsTaskConditionsCompleted() && TaskManager.Instance.CurrentTask == _addedTask.Task)
				TaskManager.Instance.CurrentTask.Complete();		
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.TryGetComponent(out Box box))
		{
			if (_addedBoxes.Contains(box))
				_addedBoxes.Remove(box);
		}
	}

	private bool IsTaskConditionsCompleted()
	{
		if (_neededBoxes.Count != _addedBoxes.Count)
			return false;

		for (int i = 0; i < _neededBoxes.Count; i++)
		{
			if (!_neededBoxes.Contains(_addedBoxes[i]))
				return false;
		}

		return true;
	}

	private void RemoveBox()
	{
		if (PlayerInventory.Instance.TryGetCurrentItem(out GameObject item) && item.TryGetComponent(out Box box))	
			_addedBoxes.Remove(box);
	}

	private void FinishMovingTask()
	{
		Debug.Log("Moving task has been completed");
	}
}
