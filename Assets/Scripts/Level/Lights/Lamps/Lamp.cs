using UnityEngine;
using UnityEngine.Events;

namespace Level.Lights.Lamp
{
	[RequireComponent(typeof(BoxCollider))] //triger zone
	public class Lamp : MonoBehaviour
	{
		[field: SerializeField] public bool IsLampEnabled { get; set; } = true;

		[SerializeField] private string _playerTag = "Player";

		[field: SerializeField] protected Light Light { get; private set; }

		[Space(10)]
		[SerializeField] private UnityEvent OnStay;

		private void OnTriggerStay(Collider other)
		{
			TryInvokeLamp(other);
		}

		protected virtual void TryInvokeLamp(Collider other)
		{
			if (!IsLampEnabled || !other.CompareTag(_playerTag))
				return;

			OnStay.Invoke();
		}

		public void SwitchLampState(bool isEnabled)
		{
			IsLampEnabled = isEnabled;

			Light.gameObject.SetActive(IsLampEnabled);
		}

		private void OnValidate()
		{
			if (Light)
				Light.gameObject?.SetActive(IsLampEnabled);
		}
	}
}
