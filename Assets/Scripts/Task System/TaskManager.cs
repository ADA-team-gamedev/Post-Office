using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
	public static TaskManager Instance { get; private set; }

	public Task CurrentTask { get; private set; } = null;

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

		foreach (var taskData in _taskDatas)
		{
			TryAddNewTask(taskData);
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

	public bool TryGetTask(int id, out Task task)
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
		Task task = new Task(taskData.Task);

		SetNewCurrentTask(task);
	}

	public void SetNewCurrentTask(Task task)
	{
		if (!TryGetTask(task.ID, out Task _))
		{
			Debug.LogWarning($"You are trying to set task({task.Name}) which doesn't exists in task collection therefore we try to add task automatically");

			if (!TryAddNewTask(task))
				return;
		}
		
		CurrentTask = task;

		OnNewCurrentTaskSet?.Invoke(task);
	}

	public bool TryAddNewTask(TaskData taskData)
	{
		Task task = new Task(taskData.Task.ID, taskData.Task.Name, taskData.Task.Description);

		return TryAddNewTask(task);
	}

	public bool TryAddNewTask(Task task) 
	{
		if (IsContainTask(task.ID))
		{
			Debug.LogWarning($"You are trying to add task({task.Name}) which already exists in task collection. We can't add him!");

			return false;
		}	

		_tasks.Add(task);

		task.OnCompleted += RemoveCurrentTask;

		OnAddedNewTask?.Invoke();

		if (CurrentTask == null)
			SetNewCurrentTask(task);

		return true;
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

		//Delete task form task collection when the player complete him
		_tasks.Remove(completedTask);

		if (TaskCount > 0)
			SetNewCurrentTask(0);

		//Just making task completed and making the text style as completed for task text in notebook
		//foreach (var item in _tasks)
		//{
		//	if (!item.IsCompleted)
		//	{
		//		SetNewCurrentTask(item);

		//		return;
		//	}
		//}

		Debug.Log($"Task: {completedTask.Name} has been completed");
	}
}
