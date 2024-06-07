using Audio;
using Events.CrushedPC;
using Items.Icon;
using Level.Lights.Lamps;
using System;
using TaskSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityModification;

namespace Level.Lights
{
	public class FuseBox : DestructiveBehaviour<FuseBox>
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
		[SerializeField] private FuseSwitch[] _switches;

		private Lamp[] _lamps;

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
				}
				else if (value <= 0)
				{
					_energyAmount = 0;

					DisableFuse();
				}
				else
				{
					_energyAmount = value;
				}
			}
		}

		private float _energyAmount;

		private uint _activatedSwitchesCount = 0;

		private void Start()
		{
			_energyAmount = _maxEnergyAmount;
			
			CountNumberOfActivatedSwitches();

			_lamps = FindObjectsOfType<Lamp>();

			SubscribePcOnFuseEvents();

			SubscribeFuseSwitchOnFuseEvents();		

			TaskManager.Instance.OnObjectDestroyed += OnTaskManagerDestroyed;
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

			foreach (var switches in _switches)
			{
				if (switches.IsEnabled)
					_activatedSwitchesCount++;
			}
		}

		[ContextMenu(nameof(DisableFuse))]
		private void DisableFuse()
		{
			if (!Application.IsPlaying(this) || !IsEnabled)
				return;

#if UNITY_EDITOR
			Debug.Log("Generator has disabled");
#endif

			IsEnabled = false;

			OnFuseDisabled?.Invoke();

			foreach (var lamp in _lamps)
			{
				lamp.CanBeEnabled = false;
			}

			AudioManager.Instance.PlaySound("Fuse Off", transform.position, spatialBlend: 0.5f);

			EnergyAmount = _maxEnergyAmount; //if we want to wait, until fuse box charging to 100%, delete this line and add enable to max energy amount property set

			GiveTask();	
		}

		[ContextMenu(nameof(EnableFuse))]
		public void EnableFuse()
		{
			if (!Application.IsPlaying(this) || IsEnabled)
				return;

#if UNITY_EDITOR
			Debug.Log("Generator has enabled");
#endif

			IsEnabled = true;

			foreach (var lamp in _lamps)
			{
				lamp.CanBeEnabled = true;
			}

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

			foreach (var fuseSwitch in _switches)
			{
				fuseSwitch.OnClickedOnSwitch += CompleteTask;
			}

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

				foreach (var fuseSwitch in _switches)
				{
					fuseSwitch.OnClickedOnSwitch -= CompleteTask;
				}
			}
		}

		#endregion

		private void OnSwitchObjectDestroyed(FuseSwitch fuseSwitch)
		{
			fuseSwitch.OnObjectDestroyed -= OnSwitchObjectDestroyed;

			fuseSwitch.OnSwitchStateChanged -= CountNumberOfActivatedSwitches;

			fuseSwitch.OnClickedOnSwitch -= CompleteTask;
		}

		private void SubscribeFuseSwitchOnFuseEvents()
		{
			foreach (var fuseSwitch in _switches)
			{
				fuseSwitch.OnObjectDestroyed += OnSwitchObjectDestroyed;

				fuseSwitch.OnSwitchStateChanged += CountNumberOfActivatedSwitches;

				OnFuseEnabled.AddListener(fuseSwitch.EnableSwitch);

				OnFuseDisabled.AddListener(fuseSwitch.DisableSwitch);
			}
		}

		private void SubscribePcOnFuseEvents()
		{
			CrashedComputerUnit[] computerUnits = FindObjectsOfType<CrashedComputerUnit>();

			foreach (var computerUnit in computerUnits)
			{
				OnFuseEnabled.AddListener(computerUnit.EnableComputer);

				OnFuseDisabled.AddListener(computerUnit.DisablePC);
			}
		}

		private void OnTaskManagerDestroyed(TaskManager taskManager)
		{
			TaskManager.Instance.OnObjectDestroyed -= OnTaskManagerDestroyed;

			TaskManager.Instance.OnNewCurrentTaskSet -= ChangeIconState;
		}

		private void OnValidate()
		{
			if (_maxEnergyAmount <= 0)
				_maxEnergyAmount++;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			OnFuseDisabled.RemoveAllListeners();

			OnFuseEnabled.RemoveAllListeners();
		}
	}
}
