using Items;
using Items.Icon;
using System.Collections.Generic;
using TaskSystem.NoteBook;
using UnityEngine;

namespace TaskSystem.TaskGivers
{
	[RequireComponent(typeof(BoxCollider))]
	public class ItemCollectorQuest : MonoBehaviour
	{
		[Header("Task")]
		[SerializeField] private string _playerTag = "Player";

		[SerializeField] private bool _giveTaskOnStart = false;
		[SerializeField] private bool _canPlayerPickUpItemAfterQuestFinishing = false;

		[SerializeField] private Tablet _noteBook;

		[Header("Hint Text")]
		[SerializeField] private string _addedItemHint = "Added Box";
		[SerializeField] private string _removedItemHint = "Removed Box";

		[Header("Zone Icon")]
		[SerializeField] private Icon _questZoneIcon;

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

		private void Update()
		{
			_questZoneIcon.RotateIconToObject();
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!_isTaskAdded && other.CompareTag(_playerTag))
				GiveTaskToPlayer();

			if (other.TryGetComponent(out Item item) && !_addedItem.Contains(item))
			{
				_addedItem.Add(item);

				_noteBook.WriteHintText(_addedItemHint, _neededItems.Contains(item) ? Color.green : Color.red);

				item.OnPickUpItem += RemoveBoxFromCollection;

				TryCompleteTask();
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.TryGetComponent(out Item item))
			{
				_noteBook.WriteHintText(_removedItemHint, _neededItems.Contains(item) ? Color.red : Color.green);

				if (_addedItem.Contains(item))
					_addedItem.Remove(item);

				TryCompleteTask();
			}
		}

		#region Item Icons Visualization

		private void ChangeQuestIconsState(Task currentTask)
		{
			if (currentTask.ID != _addedTask.Task.ID)
			{
				_questZoneIcon.HideIcon();

				foreach (var item in _neededItems)
				{
					item.ItemIcon.HideIcon();
				}

				return;
			}

			_questZoneIcon.ShowIcon();

			foreach (var item in _neededItems)
			{
				if (!item.IsPicked)
				{
					item.ItemIcon.ShowIcon(item);
				}
			}
		}

		private void OnItemDroped(Item item)
		{
			if (TaskManager.Instance.CurrentTask.ID != _addedTask.Task.ID)
			{
				item.ItemIcon.HideIcon();

				return;
			}
			
			item.ItemIcon.ShowIcon(item);
		}

		#endregion

		#region Task Completing Method

		private void GiveTaskToPlayer()
		{
			if (_isTaskAdded)
				return;

			TaskManager.Instance.OnNewCurrentTaskSet += ChangeQuestIconsState;

			TaskManager.Instance.TryAddNewTask(_addedTask);

			_isTaskAdded = true;

			foreach (Item item in _neededItems)
			{
				item.OnPickUpItem += item.ItemIcon.HideIcon;

				item.OnDropItem += OnItemDroped;
			}
		}

		private void TryCompleteTask()
		{
			if (!IsAllBoxesCollected() || !TaskManager.Instance.TryGetTask(_addedTask.Task.ID, out Task task))
				return;

			task.Complete();

			TaskManager.Instance.OnNewCurrentTaskSet -= ChangeQuestIconsState;

			foreach (Item item in _neededItems)
			{
				item.CanBePicked = _canPlayerPickUpItemAfterQuestFinishing;

				item.OnPickUpItem -= item.ItemIcon.HideIcon;

				item.OnDropItem -= OnItemDroped;

				item.ItemIcon.HideIcon();
			}
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

		private void RemoveBoxFromCollection(Item item)
		{
			_noteBook.WriteHintText(_removedItemHint, _neededItems.Contains(item) ? Color.red : Color.green);

			_addedItem.Remove(item);

			item.OnPickUpItem -= RemoveBoxFromCollection;

			TryCompleteTask();
		}

		#endregion
	}
}