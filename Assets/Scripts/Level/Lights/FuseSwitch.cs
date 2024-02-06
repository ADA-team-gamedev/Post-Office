using System;
using UnityEngine;
using UnityEngine.Events;

public class FuseSwitch : MonoBehaviour, IInteractable
{
	[field: SerializeField] public bool IsEnabled { get; private set; }

	[Space(10)]

	public UnityEvent OnSwitchEnabled;
	public UnityEvent OnSwitchDisabled;

	public event Action OnSwitchStateChanged;

	private FuseBox _generator;

	private Animator _animator;

	private void Awake()
	{
		_animator = GetComponent<Animator>();

		_generator = GetComponentInParent<FuseBox>();	
	}

	private void Start()
	{
		if (IsEnabled)
			EnableSwitch();
		else
			DisableSwitch();
	}

	public void StartInteract()
	{
		if (IsEnabled)
			DisableSwitch();
		else
			EnableSwitch();		
	}

	public void StopInteract()
	{

	}

	public void DisableSwitch()
	{
		IsEnabled = false;

		OnSwitchStateChanged?.Invoke();

		_animator.SetTrigger("OnDisable");

		OnSwitchDisabled.Invoke();
		
		_generator.OnFuseEnabled.RemoveListener(ActiveSwitchLater);
	}

	public void EnableSwitch()
	{
		IsEnabled = true;

		OnSwitchStateChanged?.Invoke();

		_animator.SetTrigger("OnEnable");

		if (_generator.IsEnabled)
			OnSwitchEnabled.Invoke();
		else
			_generator.OnFuseEnabled.AddListener(ActiveSwitchLater);
	}

	private void ActiveSwitchLater()
	{
		OnSwitchEnabled.Invoke();
	}
}
