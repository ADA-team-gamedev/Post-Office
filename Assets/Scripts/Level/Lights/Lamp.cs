using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class Lamp : MonoBehaviour
{
	public bool IsLampEnabled { get; set; } = true;

	[SerializeField] private string _playerTag = "Player";

	[SerializeField] private GameObject _spotLight;

	[SerializeField] private UnityEvent OnStay;

	private void OnTriggerStay(Collider other)
	{
		if (!IsLampEnabled || !other.CompareTag(_playerTag))
			return;
	
		OnStay.Invoke();
	}

	private void OnValidate()
	{
		_spotLight?.SetActive(IsLampEnabled);
	}

	public void SwitchLampState(bool isEnabled)
	{
		IsLampEnabled = isEnabled;

		_spotLight.SetActive(IsLampEnabled);
	}
}
