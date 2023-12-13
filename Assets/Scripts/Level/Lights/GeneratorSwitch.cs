using UnityEngine;
using UnityEngine.Events;

public class GeneratorSwitch : MonoBehaviour
{
	public bool IsEnabled { get; private set; }

	[SerializeField] private UnityEvent OnSwitchEnabled;
	[SerializeField] private UnityEvent OnSwitchDisabled;

	private GeneratorBox _generator;

	private Animator _animator;

	private void Awake()
	{
		_animator = GetComponent<Animator>();

		_generator = GetComponentInParent<GeneratorBox>();	
	}

	private void OnMouseDown()
	{
		if (IsEnabled)
			DisableSwitch();
		else
			EnableSwitch();

		_generator.CheckIsSwitchesEnabled();		
	}

	public void DisableSwitch()
	{
		IsEnabled = false;

		_animator.SetTrigger("OnDisable");

		OnSwitchDisabled.Invoke();
	}

	public void EnableSwitch()
	{
		IsEnabled = true;

		_animator.SetTrigger("OnEnable");

		OnSwitchEnabled.Invoke();
	}
}
