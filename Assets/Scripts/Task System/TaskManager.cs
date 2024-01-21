using System;
using System.Collections.Generic;
using UnityEngine;

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

	public event Action OnNewTaskSet;

	public event Action<Task> OnNewCurrentTaskSet;
	public event Action CurrentTaskCompleted;

	[SerializeField] private List<TaskData> _tasks = new();

	private void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Debug.LogWarning("TaskManager Instance already exists!");
	}

	private void Start()
	{
		foreach (var task in _tasks)
		{
			task.Task.OnCompleted += RemoveCurrentTask;
		}
	}

	public bool TryGetTaskByType(TaskType type, out TaskData taskData)
	{
		foreach (var item in _tasks)
		{
			if (item.Task.Type == type)
			{
				taskData = item;
				
				return true;
			}				
		}

		taskData = null;

		return false;
	}

	public void SetNewCurrentTask(TaskData taskData)
	{
		if (!TryGetTaskByType(taskData.Task.Type, out TaskData _))
		{
			Debug.LogWarning("You are trying to set task which doesn't exists in task collection therefore we adding task automatically");

			TryAddNewTask(taskData);
		}

		CurrentTask = taskData.Task;

		OnNewCurrentTaskSet?.Invoke(taskData.Task);
	}

	public void SetNewCurrentTask(TaskType type)
	{
		if (!TryGetTaskByType(type, out TaskData taskData))
		{
			Debug.LogWarning("You are trying to set task which doesn't exists in task collection therefore we adding task automatically");

			TryAddNewTask(taskData);
		}

		CurrentTask = taskData.Task;

		OnNewCurrentTaskSet?.Invoke(taskData.Task);
	}

	public bool TryAddNewTask(TaskData taskData) 
	{
		if (IsContainTaskByType(taskData.Task.Type))
		{
			Debug.LogWarning("You are trying to add task which already exists in task collection");

			return false;
		}

		_tasks.Add(taskData);

		taskData.Task.OnCompleted += RemoveCurrentTask;

		OnNewTaskSet?.Invoke();

		return true;
	}

	private bool IsContainTaskByType(TaskType type)
	{
		foreach (var task in _tasks)
		{
			if (task.Task.Type == type)
				return true;
		}

		return false;
	}

	private void RemoveCurrentTask()
	{
		for (int i = 0; i < _tasks.Count; i++)
		{
			if (_tasks[i].Task.IsCompleted)
			{
				if (CurrentTask == _tasks[i].Task)
					CurrentTask = null;

				CurrentTaskCompleted?.Invoke();

				_tasks.Remove(_tasks[i]);		
			}
		}

		Debug.Log("Task has been deleted");
	}
}
