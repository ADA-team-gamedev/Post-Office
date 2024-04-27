using System;
using UnityEngine;
using UnityEngine.Events;

namespace Level.Lights.Lamp
{
	[RequireComponent(typeof(BoxCollider))] //triger zone
	[SelectionBase]
	public class Lamp : MonoBehaviour, IEvent
	{
		[field: SerializeField] public bool IsLampEnabled { get; private set; } = true;

		[SerializeField] private string _playerTag = "Player";

		[field: SerializeField] protected Light Light { get; private set; }

		[field: SerializeField] protected Renderer LampRenderer { get; private set; }

		protected MaterialPropertyBlock MaterialPropertyBlock { get; set; }

		protected const string EmissionColor = "_EmissionColor";

		protected Color DefaultLampColor { get; private set; } = Color.white;

		protected Color DisabledLampColor { get; private set; } = Color.black;

		[Space(10)]
		[SerializeField] private UnityEvent OnStay;

		public event Action<bool> OnLampStateChanged;

		public event Action<Lamp> OnObjectDestroyed;

		protected virtual void Awake()
		{
			InitializeLamp();
		}

		protected virtual void InitializeLamp()
		{
			MaterialPropertyBlock = new();
		}

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
			if (!gameObject.activeInHierarchy)
				return;

			OnLampStateChanged?.Invoke(isEnabled);

			IsLampEnabled = isEnabled;

			Light.gameObject.SetActive(IsLampEnabled);

			Color currentEmissionColor = IsLampEnabled ? DefaultLampColor : DisabledLampColor;

			MaterialPropertyBlock.SetColor(EmissionColor, currentEmissionColor);

			LampRenderer.SetPropertyBlock(MaterialPropertyBlock);
		}

		public void PlayEvent()
		{
			SwitchLampState(IsLampEnabled);
		}

		protected virtual void OnValidate()
		{
			MaterialPropertyBlock ??= new();

			if (IsLampEnabled)
				SwitchLampState(IsLampEnabled);
		}

		protected virtual void OnDestroy()
		{
			OnObjectDestroyed?.Invoke(this);
		}
	}
}
