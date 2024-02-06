using UnityEngine;
using UnityEngine.Events;

public class FuseBox : MonoBehaviour
{
	public bool IsEnabled { get; private set; } = true;

	[Header("Energy")]

	[SerializeField][Range(1f, 100f)] private float _energyIncreasingSpeed = 1f;
	[SerializeField][Range(0.1f, 5f)] private float _energyDecreasingSpeed = 1f;

	[SerializeField] private float _maxEnergyAmount = 100f;

	[Header("Task")]

	[SerializeField] private TaskData _task;
	private bool _isTaskAdded = false;
	private bool _isTaskCompleted = false;

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

				EnableFuse();
			}
			else if (value <= 0)
			{
				_energyAmount = 0;

				IsEnabled = false;

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

		foreach (var switches in generatorSwitches)
		{
			switches.OnSwitchStateChanged += CountNumberOfActivatedSwitches;
		}
	}

	private void Update()
	{
		if (IsEnabled)
			DecreaseEnergy();
		else
			IncreaseEnergy();
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

		if (!_isTaskAdded)
		{
			TaskManager.Instance.SetNewCurrentTask(_task);

			_isTaskAdded = true;
		}
	}

	private void EnableFuse()
	{
		IsEnabled = true;

		OnFuseEnabled?.Invoke();

		CompleteTask();	
	}

	private void CompleteTask()
	{
		if (_isTaskCompleted)
			return;

		if (_activatedSwitchesCount > 0 && TaskManager.Instance.TryGetTaskByType(_task.Task.ID, out Task task))
		{
			task.Complete();

			_isTaskCompleted = true;
		}
	}

	private void OnValidate()
	{
		if (_maxEnergyAmount <= 0)
			_maxEnergyAmount++;
	}
}
