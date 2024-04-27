using Events.CrushedPC;
using UnityEngine;

namespace TaskSystem.TaskGivers
{
    public class CrashedPCQuest : MonoBehaviour
    {
		[SerializeField] private TaskData _crushedPCTask;

        private CrushedComputerUnit[] _crushedComputers;

		private void Start()
		{
			FillPC();

			foreach (var computer in _crushedComputers)
			{
				computer.OnObjectDestroyed += OnComputerUnitDestroyed;

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
			_crushedComputers = FindObjectsOfType<CrushedComputerUnit>();
		}

		private void OnComputerUnitDestroyed(CrushedComputerUnit computerUnit)
		{
			computerUnit.OnObjectDestroyed -= OnComputerUnitDestroyed;

			computerUnit.OnPCCrushed -= GiveTaskToPlayer;

			computerUnit.OnPCFixed -= OnPlayerFixPC;
		}
	}
}