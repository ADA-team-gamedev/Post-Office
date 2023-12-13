using UnityEngine;
using UnityEngine.Events;

public class GeneratorBox : MonoBehaviour
{
	[SerializeField] private bool GenerateRandomSwitchState = true;

    [SerializeField] private UnityEvent OnGeneratorDisabled;
	[SerializeField] private UnityEvent OnGeneratorEnabled;

    [SerializeField] private GeneratorSwitch[] generatorSwitches;

	private void Start()
	{
		if (GenerateRandomSwitchState)
			CreateRandomSwitchEnabled();
		else 
			DisableAllSwitches();

		CheckIsSwitchesEnabled();
	}

	private void Update()
	{
		
	}

	public void CheckIsSwitchesEnabled()
	{
		foreach (var switches in generatorSwitches)
		{
			if (!switches.IsEnabled)
			{
				OnGeneratorDisabled.Invoke();

				return;
			}
		}

		OnGeneratorEnabled.Invoke();
	}

	public void DisableAllSwitches()
	{
		foreach (var switches in generatorSwitches)
			switches.DisableSwitch();		
	}

	private void CreateRandomSwitchEnabled()
	{
		foreach (var switches in generatorSwitches)
		{
			if (Random.Range(0, 2) == 0)
				switches.DisableSwitch();
			else
				switches.EnableSwitch();
		}
	}
}
