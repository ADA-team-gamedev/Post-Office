using UnityEngine;
using UnityEngine.Events;

public class GeneratorBox : MonoBehaviour
{
	public bool IsEnabled { get; private set; } = true;

    public UnityEvent OnGeneratorDisabled;
	public UnityEvent OnGeneratorEnabled;

    [SerializeField] private GeneratorSwitch[] generatorSwitches;

	[SerializeField][Range(1f, 100f)] private float _energyIncreasingSpeed = 1f;
	[SerializeField][Range(1f, 5f)] private float _energyDecreasingSpeed = 1f;

	[SerializeField] private float _maxEnergyAmount = 100f;

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

				IsEnabled = true;
			}
			else if (value <= 0)
			{
				_energyAmount = 0;

				IsEnabled = false;

				DisableAllSwitches();
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
			switches.OnSwitchEnabled.AddListener(AddSwitchToAll);

			switches.OnSwitchDisabled.AddListener(RemoveSwitchFromAll);
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

	private void RemoveSwitchFromAll()
	{
		_activatedSwitchesCount--;
	}

	private void AddSwitchToAll()
	{
		_activatedSwitchesCount++;
	}

	public void DisableAllSwitches()
	{
		foreach (var switches in generatorSwitches)
			switches.DisableSwitch();		
	}

	private void OnValidate()
	{
		if (EnergyAmount <= 0)
			EnergyAmount++;
	}
}
