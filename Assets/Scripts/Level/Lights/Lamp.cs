using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class Lamp : MonoBehaviour
{
	[field: SerializeField] public bool IsLampEnabled = true;

	[SerializeField] private GameObject _spotLight;

	[SerializeField] private UnityEvent OnStay;

	private void OnTriggerStay(Collider other)
	{
		if (!IsLampEnabled)
			return;

		OnStay.Invoke();
	}

	private void OnValidate()
	{
		_spotLight?.SetActive(IsLampEnabled);
	}
}
