using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.GlobalIllumination;

[RequireComponent(typeof(BoxCollider))]
public class Lamp : MonoBehaviour
{
	[field: SerializeField] public bool IsLampEnabled = true;

	[SerializeField] private GameObject _spotLight;

    public UnityEvent OnEnter;
	public UnityEvent OnStay;
    public UnityEvent OnExit;

	private void OnTriggerEnter(Collider other)
	{
		if (!IsLampEnabled)
			return;

		OnEnter.Invoke();
	}

	private void OnTriggerStay(Collider other)
	{
		if (!IsLampEnabled)
			return;

		OnStay.Invoke();
	}

	private void OnTriggerExit(Collider other)
	{
		if (!IsLampEnabled)
			return;

		OnExit.Invoke();
	}

	private void OnValidate()
	{
		_spotLight?.SetActive(IsLampEnabled);
	}
}
