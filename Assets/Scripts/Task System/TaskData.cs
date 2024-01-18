using System;
using UnityEngine;

[CreateAssetMenu]
public class TaskData : ScriptableObject
{
	[field: SerializeField] public Task Task { get; private set; }
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
			Debug.LogWarning($"Task is already completed but you still trying to complete him");

		IsCompleted = true;

		OnCompleted?.Invoke();
	}
}
