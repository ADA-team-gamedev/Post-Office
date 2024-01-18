using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SortQuest : MonoBehaviour
{
	[SerializeField] private Task _addedTask = new(TaskType.BoxMoving, "Розчистка території", "Віднесіть задані коробки до пункту прийому");

	[SerializeField] private List<Box> _neededBoxes;
	
	private List<Box> _addedBoxes = new();

	[SerializeField] private TextMeshProUGUI text;

	private void Start()
	{
		if (TaskManager.Instance.TryGetTaskByType(_addedTask.Type, out Task neededTask))
			neededTask.OnCompleted += FinishMovingTask;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && TaskManager.Instance.CurrentTask != _addedTask)
		{
			TaskManager.Instance.TryAddNewTask(_addedTask);

			TaskManager.Instance.SetNewCurrentTask(_addedTask);
			
			TaskManager.Instance.CurrentTask.OnCompleted += FinishMovingTask;

			text.text = TaskManager.Instance.CurrentTask.Description;
		}

		if (other.TryGetComponent(out Box box) && !_addedBoxes.Contains(box))
		{
			_addedBoxes.Add(box);

			if (IsTaskConditionsCompleted() && TaskManager.Instance.CurrentTask == _addedTask)
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

	private void FinishMovingTask()
	{
		Debug.Log("Moving task has been completed");
	}
}
