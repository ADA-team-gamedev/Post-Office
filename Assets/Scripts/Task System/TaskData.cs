using System;
using UnityEngine;

[CreateAssetMenu]
public class TaskData : ScriptableObject
{
	[field: SerializeField] public Task Task { get; private set; }
}

public enum TaskType
{
	Sorting,
	BoxMoving,
	Finding,
}

[Serializable]
public class Task
{
	public bool IsCompleted { get; private set; } = false;

	[field: SerializeField] public TaskType Type { get; private set; }

	[field: SerializeField, TextArea(2, 2)] public string Name { get; private set; }

	[field: SerializeField, TextArea(5, 5)] public string Description { get; private set; }

	public event Action<Task> OnCompleted;

	public Task(TaskType type, string name, string description)
	{
		Type = type;

		Name = name;

		Description = description;
	}

	public Task(TaskData taskData)
	{
		Type = taskData.Task.Type;

		Name = taskData.Task.Name;

		Description = taskData.Task.Description;
	}

	public void Complete()
	{
		if (IsCompleted)
			Debug.LogWarning($"Task is already completed but you still trying to complete him");

		IsCompleted = true;

		OnCompleted?.Invoke(this);
	}
}
