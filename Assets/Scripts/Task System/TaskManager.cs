using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum TaskType
{
    Sorting,
    BoxMoving,
}

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
		SetNewCurrentTask(_tasks[0]);	
	}

	public bool TryGetTaskByType(TaskType type, out Task task)
	{
		foreach (var item in _tasks)
		{
			if (item.Type == type)
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
			Debug.LogWarning("You are trying to set new task as current with using wrong index!");

			return;
		}

		Task task = _tasks[index];

		CurrentTask = task;

		OnNewCurrentTaskSet?.Invoke(task);
	}

	public void SetNewCurrentTask(Task task)
	{
		if (!TryGetTaskByType(task.Type, out Task _))
		{
			Debug.LogWarning("You are trying to set task which doesn't exists in task collection therefore we adding task automatically");

			TryAddNewTask(task);
		}

		CurrentTask = task;

		OnNewCurrentTaskSet?.Invoke(task);
	}

	public void SetNewCurrentTask(TaskType type)
	{
		if (!TryGetTaskByType(type, out Task task))
		{
			Debug.LogError("You are trying to set task by type which doesn't exists in task collection. We can't set it as current");
		}

		CurrentTask = task;

		OnNewCurrentTaskSet?.Invoke(task);
	}

	public bool TryAddNewTask(Task task) 
	{
		if (IsContainTaskByType(task.Type))
		{
			Debug.LogWarning("You are trying to add task which already exists in task collection");

			return false;
		}

		_tasks.Add(task);

		task.OnCompleted += RemoveCurrentTask;

		OnAddedNewTask?.Invoke();

		return true;
	}

	private bool IsContainTaskByType(TaskType type)
	{
		foreach (var task in _tasks)
		{
			if (task.Type == type)
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

		_tasks.Remove(completedTask);

		if (_tasks.Count >= 0)
			SetNewCurrentTask(_tasks[0]);

		Debug.Log($"Task: {completedTask.Name} has been deleted");
	}
}
