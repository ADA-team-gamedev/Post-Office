using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class Lamp : MonoBehaviour
{
	[field: SerializeField] public bool IsLampEnabled = true;

	[SerializeField] private GameObject _lamp;

    public UnityEvent OnEnter;
	public UnityEvent OnStay;
    public UnityEvent OnExit;

	private void OnTriggerEnter(Collider other)
	{
		OnEnter.Invoke();
	}

	private void OnTriggerStay(Collider other)
	{
		OnStay.Invoke();
	}

	private void OnTriggerExit(Collider other)
	{
		OnExit.Invoke();
	}

	private void OnValidate()
	{
		_lamp?.SetActive(IsLampEnabled);
	}
}
