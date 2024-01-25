using System.Collections.Generic;
using UnityEngine;

public class SortQuest : MonoBehaviour
{
	[SerializeField] private TaskData _addedTask;

	[SerializeField] private List<Box> _neededBoxes;
	
	private List<Box> _addedBoxes = new();

	[SerializeField] private NoteBook _noteBook;

	private bool _isTaskAdded = false;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && !_isTaskAdded && TaskManager.Instance.CurrentTask != _addedTask.Task)
		{		
			TaskManager.Instance.SetNewCurrentTask(_addedTask.Task);

			_isTaskAdded = true;

			_noteBook.AddExtraText("Необхідно перенести деякі коробки нижче на склад, на полицю, біля автівок. Ця партія коробок має відправитись дуже скоро: \n- Коробка зі склом\n- Коробка з книгами");		
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

			if (IsTaskConditionsCompleted() && TaskManager.Instance.CurrentTask == _addedTask.Task)
				TaskManager.Instance.CurrentTask.Complete();
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
		if (PlayerInventory.Instance.TryGetCurrentItem(out Box box))	
			_addedBoxes.Remove(box);
	}
}
