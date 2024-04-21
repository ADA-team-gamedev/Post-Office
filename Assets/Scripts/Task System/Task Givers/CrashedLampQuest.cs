using Level.Lights.Lamp;
using UnityEngine;

namespace TaskSystem.TaskGivers
{
	public class CrashedLampQuest : MonoBehaviour
	{
		[Header("Task")]
		[SerializeField] private TaskData _crashedLampTask;
		
		private BreakableLamp[] _breakableLamps;

		private void Start()
		{
			FillLamps();

			foreach (var lamp in _breakableLamps)
			{
				lamp.OnLampDestroyed += GiveTaskToPlayer;
			}
		}

		private void GiveTaskToPlayer()
		{
			if (_breakableLamps.Length <= 0 || !TaskManager.Instance.TryAddNewTask(_crashedLampTask))
			{
				Debug.LogWarning("We can't add crashed lamp task!");

				return;
			}

			foreach (var lamp in _breakableLamps)
			{
				lamp.OnLampDestroyed -= GiveTaskToPlayer;

				lamp.OnLampFixed += OnPlayerFixedLamp;
			}
		}

		private void OnPlayerFixedLamp()
		{
			foreach (var lamp in _breakableLamps)
			{
				lamp.OnLampFixed -= OnPlayerFixedLamp;
			}

			if (!TaskManager.Instance.TryGetTask(_crashedLampTask.Task.ID, out Task task))
				return;

			task.Complete();
		}

		private void FillLamps()
		{
			_breakableLamps = FindObjectsOfType<BreakableLamp>();
		}
	}
}