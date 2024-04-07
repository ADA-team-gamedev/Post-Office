using System;
using System.Linq;
using System.Collections.Generic;
using TaskSystem.NoteBook;
using UnityEngine;

namespace TaskSystem
{
	public class TaskManager : MonoBehaviour
	{
		#region Task System

		[field: Header("Tassk System")]

		public static TaskManager Instance { get; private set; }

		public Task CurrentTask { get; private set; } = null;

		public int TaskCount => _tasks.Count;

		#region Actions

		public event Action OnAddedNewTask;

		public event Action OnTaskCompleted;

		public event Action<Task> OnNewCurrentTaskSet;
		public event Action OnCurrentTaskCompleted;

		#endregion

		[SerializeField] private List<TaskData> _taskDatas = new();

		private Dictionary<int, Task> _tasks = new();

		#endregion

		[SerializeField] private Tablet _tablet;

		private void Awake()
		{
			if (Instance == null)
				Instance = this;
			else
				Debug.LogWarning($"{this} Instance already exists!");

			_tablet.SubcribeOnTaskManager();

			foreach (var taskData in _taskDatas)
			{
				TryAddNewTask(taskData);
			}

			foreach (var task in _tasks)
			{
				task.Value.OnCompleted += CompleteTask;
			}
		}

		private void Start()
		{
			if (_tasks.Count > 0)
				SetNewCurrentTask(_tasks.Keys.ElementAt(0));
		}

		public bool TryGetTask(int id, out Task task)
			=> _tasks.TryGetValue(id, out task);

		public void SetNewCurrentTask(int index)
		{
			if (index < 0 || index >= _tasks.Count)
			{
				Debug.LogWarning($"Can't set task, as current with index[{index}]");

				return;
			}

			Task task = _tasks.Values.ElementAt(index);

			CurrentTask = task;

			OnNewCurrentTaskSet?.Invoke(task);
		}

		public void SetNewCurrentTask(TaskData taskData)
		{
			Task task = new Task(taskData.Task);

			SetNewCurrentTask(task);
		}

		public void SetNewCurrentTask(Task task)
		{
			if (!TryGetTask(task.ID, out Task _))
			{
				Debug.LogWarning($"You are trying to set task({task.Name}) which doesn't exists in task collection therefore we try to add task automatically");

				if (!TryAddNewTask(task))
				{
					Debug.LogWarning($"Couldn't add this task({task.Name}!)");

					return;
				}
			}

			CurrentTask = task;

			OnNewCurrentTaskSet?.Invoke(task);
		}

		public bool TryAddNewTask(TaskData taskData)
		{
			Task task = new Task(taskData.Task);

			return TryAddNewTask(task);
		}

		public bool TryAddNewTask(Task task)
		{
			if (IsContainsTask(task.ID))
			{
				Debug.LogWarning($"You are trying to add task({task.Name}) which already exists in task collection. We can't add him!");

				return false;
			}

			_tasks.Add(task.ID, task);

			task.OnCompleted += CompleteTask;
			
			OnAddedNewTask?.Invoke();

			if (CurrentTask == null)
				SetNewCurrentTask(task);

			return true;
		}

		private bool IsContainsTask(int taskId)
			=> _tasks.ContainsKey(taskId);	

		private void CompleteTask(Task completedTask)
		{
			_tasks.Remove(completedTask.ID);

			bool isCompleteTaskIsCurrent = CurrentTask.ID == completedTask.ID;

			if (isCompleteTaskIsCurrent)
				OnCurrentTaskCompleted?.Invoke();

			OnTaskCompleted?.Invoke();

			if (TaskCount > 0 && isCompleteTaskIsCurrent)
				SetNewCurrentTask(0);

			Debug.Log($"Task: {completedTask.Name} has been completed");
		}
	}
}