using System;
using UnityEngine;
using UnityEngine.Events;

public class GeneratorSwitch : MonoBehaviour
{
	[field: SerializeField] public bool IsEnabled { get; private set; }

	public UnityEvent OnSwitchEnabled;
	public UnityEvent OnSwitchDisabled;

	private GeneratorBox _generator;

	private Animator _animator;

	private void Awake()
	{
		_animator = GetComponent<Animator>();

		_generator = GetComponentInParent<GeneratorBox>();	
	}

	private void Start()
	{
		if (IsEnabled)
			_animator.SetTrigger("OnEnable");
		else
			_animator.SetTrigger("OnDisable");
	}

	private void OnMouseDown()
	{
		if (IsEnabled)
			DisableSwitch();
		else
			EnableSwitch();		
	}

	public void DisableSwitch()
	{
		if (!IsEnabled)
			return;

		IsEnabled = false;

		_animator.SetTrigger("OnDisable");

		OnSwitchDisabled.Invoke();

		_generator.OnGeneratorEnabled.RemoveListener(ActiveSwitchLater);
	}

	public void EnableSwitch()
	{
		if (IsEnabled)
			return;

		IsEnabled = true;

		_animator.SetTrigger("OnEnable");

		if (_generator.IsEnabled)
			OnSwitchEnabled.Invoke();
		else
		{
			_generator.OnGeneratorEnabled.AddListener(ActiveSwitchLater);
		}
	}

	private void ActiveSwitchLater()
	{
		if (IsEnabled)
			OnSwitchEnabled.Invoke();
	}
}
