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
    
    private List<Task> _tasks = new()
	{
		{ new(TaskType.BoxMoving, "Розчистка території", "Віднесіть задані коробки до пункту прийому") },
		{ new(TaskType.Sorting, "Сортування", "Відсортуйте коробки за позначками температур") },
	};

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
			task.OnCompleted += RemoveTask;
		}
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

	public void SetNewCurrentTask(Task task)
	{
		if (!TryGetTaskByType(task.Type, out Task _))
		{
			Debug.LogWarning("You are trying to set task which doesn't exists in task collection therefore we adding task automatically");

			TryAddNewTask(task);
		}

		CurrentTask = task;

		OnNewTaskSet?.Invoke();
	}

	public void SetNewCurrentTask(TaskType type)
	{
		if (!TryGetTaskByType(type, out Task task))
		{
			Debug.LogWarning("You are trying to set task which doesn't exists in task collection therefore we adding task automatically");

			TryAddNewTask(task);
		}

		CurrentTask = task;

		OnNewTaskSet?.Invoke();
	}

	public bool TryAddNewTask(Task task) 
	{
		if (IsContainTaskByType(task.Type))
		{
			Debug.LogWarning("You are trying to add task which already exists in task collection");

			return false;
		}

		_tasks.Add(task);

		task.OnCompleted += RemoveTask;

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

	private void RemoveTask()
	{
		for (int i = 0; i < _tasks.Count; i++)
		{
			if (_tasks[i].IsCompleted)
			{
				if (CurrentTask == _tasks[i])
					CurrentTask = null;

				_tasks.Remove(_tasks[i]);		
			}
		}

		Debug.Log("Task has been deleted");
	}
}

[Serializable]
public class Task
{
    public bool IsCompleted { get; private set; } = false;

	[field: SerializeField] public TaskType Type { get; private set; }

	[field: SerializeField, TextArea] public string Name { get; private set; }

	[field: SerializeField, TextArea] public string Description { get; private set; }

    public event Action OnCompleted;

	public Task(TaskType type, string name, string description = "No description")
	{
		Type = type;

		Name = name;

		Description = description;
	}

    public void Complete()
    {
        if (IsCompleted)
            Debug.LogWarning($"Task is already comleted but you still trying to complete him");

		IsCompleted = true;

        OnCompleted?.Invoke();
	}
}
