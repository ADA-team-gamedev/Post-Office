using Events.CrushedPC;
using UnityEngine;
using UnityEngine.Modification;

namespace TaskSystem.TaskGivers
{
    public class CrashedPCQuest : DestructiveBehaviour<CrashedPCQuest>
	{
		[SerializeField] private TaskData _crushedPCTask;

        private CrashedComputerUnit[] _crushedComputers;

		private void Start()
		{
			FillPC();

			foreach (var computer in _crushedComputers)
			{
				computer.OnObjectDestroyed += OnComputerUnitsDestroyed;

				computer.OnPCCrushed += GiveTaskToPlayer;
			}
		}

		private void GiveTaskToPlayer()
		{
			if (!TaskManager.Instance.TryAddNewTask(_crushedPCTask))
				return;

			foreach (var computer in _crushedComputers)
			{
				computer.OnPCCrushed -= GiveTaskToPlayer;

				computer.OnPCFixed += OnPlayerFixPC;
			}	
		}

		private void OnPlayerFixPC()
		{
			if (!TaskManager.Instance.TryGetTask(_crushedPCTask.Task.ID, out Task task))
				return;

			task.Complete();

			foreach (var computer in _crushedComputers)
			{
				computer.OnPCFixed -= OnPlayerFixPC;
			}		
		}

		private void FillPC()
		{
			_crushedComputers = FindObjectsOfType<CrashedComputerUnit>();
		}

		private void OnComputerUnitsDestroyed(CrashedComputerUnit computerUnit)
		{
			computerUnit.OnObjectDestroyed -= OnComputerUnitsDestroyed;

			computerUnit.OnPCCrushed -= GiveTaskToPlayer;

			computerUnit.OnPCFixed -= OnPlayerFixPC;			
		}
	}
}