using System;
using System.Linq;
using System.Collections.Generic;
using TaskSystem.NoteBook;
using UnityEngine;
using Zenject;

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

		[Inject] private Tablet _tablet;

		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			else
			{
#if UNITY_EDITOR
				Debug.LogWarning($"{this} Instance already exists!");
#endif
			}

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
			if (CurrentTask == null && _tasks.Count > 0)
				SetNewCurrentTask(0);
		}

		public bool TryGetTask(int id, out Task task)
			=> _tasks.TryGetValue(id, out task);

		public void SetNewCurrentTask(int index)
		{
			if (index < 0 || index >= _tasks.Count)
			{
#if UNITY_EDITOR
				Debug.LogWarning($"Can't set task, as current with index[{index}]");
#endif
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
#if UNITY_EDITOR
				Debug.LogWarning($"You are trying to set task({task.Name}) which doesn't exists in task collection therefore we try to add task automatically");
#endif
				if (!TryAddNewTask(task))
				{
#if UNITY_EDITOR
					Debug.LogWarning($"Couldn't add this task({task.Name}!)");
#endif
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
#if UNITY_EDITOR
				Debug.LogWarning($"You are trying to add task({task.Name}) which already exists in task collection. We can't add him!");
#endif
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

#if UNITY_EDITOR
			Debug.Log($"Task: {completedTask.Name} has been completed");
#endif
		}

		private void OnDestroy()
		{
			foreach (var task in _tasks)
			{
				task.Value.OnCompleted -= CompleteTask;
			}
		}
	}
}