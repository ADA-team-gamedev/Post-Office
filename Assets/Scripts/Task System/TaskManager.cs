using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
	public static TaskManager Instance { get; private set; }

	public Task CurrentTask { get; private set; }

	public int TaskCount => _tasks.Count;

	#region Actions

	public event Action OnAddedNewTask;

	public event Action<Task> OnNewCurrentTaskSet;
	public event Action CurrentTaskCompleted;

	#endregion

	[SerializeField] private List<TaskData> _taskDatas = new();

	private List<Task> _tasks = new();

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Debug.LogWarning("TaskManager Instance already exists!");

		foreach (var item in _taskDatas)
		{
			_tasks.Add(item.Task);
		}

		foreach (var task in _tasks)
		{
			task.OnCompleted += RemoveCurrentTask;
		}
	}

	private void Start()
	{
		if (_tasks.Count > 0)
			SetNewCurrentTask(_tasks[0]);	
	}

	public bool TryGetTaskByType(int id, out Task task)
	{
		foreach (var item in _tasks)
		{
			if (item.ID == id)
			{
				task = item;
				
				return true;
			}				
		}

		task = null;

		return false;
	}

	public void SetNewCurrentTask(int index)
	{
		if (index < 0 || index >= _tasks.Count)
		{
			Debug.LogWarning($"Can't set task, as current with index[{index}]");

			return;
		}

		Task task = _tasks[index];

		CurrentTask = task;

		OnNewCurrentTaskSet?.Invoke(task);
	}

	public void SetNewCurrentTask(TaskData taskData)
	{
		Task task = new(taskData);

		SetNewCurrentTask(task);
	}

	public void SetNewCurrentTask(Task task)
	{
		if (!TryGetTaskByType(task.ID, out Task _))
		{
			Debug.LogWarning($"You are trying to set task({task.Name}) which doesn't exists in task collection therefore we adding task automatically");

			AddNewTask(task);
		}

		CurrentTask = task;

		OnNewCurrentTaskSet?.Invoke(task);
	}

	public void AddNewTask(TaskData taskData)
	{
		Task task = new(taskData);

		AddNewTask(task);
	}

	public void AddNewTask(Task task) 
	{
		if (IsContainTask(task.ID))
		{
			Debug.LogWarning($"You are trying to add task({task.Name}) which already exists in task collection. We can't add him!");

			return;
		}

		_tasks.Add(task);

		task.OnCompleted += RemoveCurrentTask;

		OnAddedNewTask?.Invoke();

		if (CurrentTask == null)
			SetNewCurrentTask(task);
	}

	private bool IsContainTask(int taskId)
	{
		foreach (var task in _tasks)
		{
			if (task.ID == taskId)
				return true;
		}

		return false;
	}

	private void RemoveCurrentTask(Task completedTask)
	{
		if (CurrentTask != completedTask)
			return;

		CurrentTask = null;

		CurrentTaskCompleted?.Invoke();

		//_tasks.Remove(completedTask);

		foreach (var item in _tasks)
		{
			if (!item.IsCompleted)
			{
				SetNewCurrentTask(item);

				return;
			}
		}

		Debug.Log($"Task: {completedTask.Name} has been completed");
	}
}
