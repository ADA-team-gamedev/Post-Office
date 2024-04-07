using Audio;
using Items.Icon;
using TaskSystem;
using UnityEngine;
using UnityEngine.Events;

namespace Level.Lights
{
	public class FuseBox : MonoBehaviour
	{
		public bool IsEnabled { get; private set; } = true;

		[Header("Energy")]

		[SerializeField][Range(1f, 100f)] private float _energyIncreasingSpeed = 1f;
		[SerializeField][Range(0.01f, 2f)] private float _energyDecreasingSpeed = 0.5f;

		[SerializeField] private float _maxEnergyAmount = 100f;

		[Header("Task")]

		[SerializeField] private TaskData _taskData;
		private bool _isTaskAdded = false;
		private bool _isTaskCompleted = false;

		[Header("Icon")]

		[SerializeField] private Icon _fuseIconForTask;

		[Header("Switches")]
		[SerializeField] private FuseSwitch[] generatorSwitches;

		[Header("Events")]
		[Space(5)]

		public UnityEvent OnFuseDisabled;
		public UnityEvent OnFuseEnabled;

		public float EnergyAmount
		{
			get
			{
				return _energyAmount;
			}
			private set
			{
				if (value >= _maxEnergyAmount)
				{
					_energyAmount = _maxEnergyAmount;

					Debug.Log("Generator has charged");

					EnableFuse();
				}
				else if (value <= 0)
				{
					_energyAmount = 0;

					IsEnabled = false;

					Debug.Log("Generator has disabled");

					DisableFuse();
				}
				else
					_energyAmount = value;
			}
		}

		private float _energyAmount;

		private uint _activatedSwitchesCount = 0;

		private void Start()
		{
			_energyAmount = _maxEnergyAmount;	

			CountNumberOfActivatedSwitches();

			foreach (var fuseSwitch in generatorSwitches)
			{
				fuseSwitch.OnSwitchStateChanged += CountNumberOfActivatedSwitches;

				fuseSwitch.OnSwitchEnabled.AddListener(CompleteTask);
			}
		}

		private void Update()
		{
			if (IsEnabled)
				DecreaseEnergy();
			else
				IncreaseEnergy();

			_fuseIconForTask.RotateIconToObject();
		}

		private void DecreaseEnergy()
		{
			if (_activatedSwitchesCount == 0)
				return;

			EnergyAmount -= _activatedSwitchesCount * _energyDecreasingSpeed * Time.deltaTime;
		}

		private void IncreaseEnergy()
		{
			EnergyAmount += _energyIncreasingSpeed * Time.deltaTime;
		}

		private void CountNumberOfActivatedSwitches()
		{
			_activatedSwitchesCount = 0;

			foreach (var switches in generatorSwitches)
			{
				if (switches.IsEnabled)
					_activatedSwitchesCount++;
			}
		}

		private void DisableFuse()
		{
			foreach (var switches in generatorSwitches)
				switches.DisableSwitch();

			OnFuseDisabled?.Invoke();

			AudioManager.Instance.PlaySound("Fuse Off", transform.position, spatialBlend: 0.5f);

			GiveTask();	
		}

		private void EnableFuse()
		{
			IsEnabled = true;

			OnFuseEnabled?.Invoke();
		}

		#region Logic for task

		private void ChangeIconState(Task currentTask)
		{
			if (currentTask.ID != _taskData.Task.ID)
			{
				_fuseIconForTask.HideIcon();

				return;
			}

			_fuseIconForTask.ShowIcon();
		}

		private void GiveTask()
		{
			if (_isTaskAdded)
				return;

			TaskManager.Instance.SetNewCurrentTask(_taskData);

			TaskManager.Instance.OnNewCurrentTaskSet += ChangeIconState;

			_isTaskAdded = true;
		}

		private void CompleteTask()
		{
			if (_isTaskCompleted)
				return;

			if (_activatedSwitchesCount > 0 && TaskManager.Instance.TryGetTask(_taskData.Task.ID, out Task task))
			{
				task.Complete();

				_isTaskCompleted = true;

				foreach (var fuseSwitch in generatorSwitches)
				{
					fuseSwitch.OnSwitchEnabled.RemoveListener(CompleteTask);
				}
			}
		}

		#endregion

		private void OnValidate()
		{
			if (_maxEnergyAmount <= 0)
				_maxEnergyAmount++;
		}
	}
}
