using Player;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityModification;

namespace Level.Lights.Lamps
{
	[RequireComponent(typeof(BoxCollider))] //triger zone
	[SelectionBase]
	public class Lamp : DestructiveBehaviour<Lamp>, IEvent
	{
		#region Lamp Settings

		[field: SerializeField] public bool IsLampEnabled { get; private set; } = true;

		[SerializeField] private string _playerTag = "Player";

		[field: SerializeField] protected Light Light { get; private set; }

		public bool CanBeEnabled { get; set; } = true;

		#endregion

		#region Renderer

		[field: SerializeField] protected Renderer LampRenderer { get; private set; }

		protected MaterialPropertyBlock MaterialPropertyBlock { get; set; }

		protected const string EmissionColor = "_EmissionColor";

		protected Color DefaultLampColor { get; private set; } = Color.white;

		protected Color DisabledLampColor { get; private set; } = Color.black;

		#endregion

		#region Lamp events

		[Space(10)]
		[SerializeField] private UnityEvent OnStay;

		public event Action<bool> OnLampStateChanged;

		#endregion

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

		public virtual void SwitchLampState(bool isEnabled)
		{
			if (!gameObject.activeInHierarchy || !CanBeEnabled)
				return;

			StopAllCoroutines();

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

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}
	}
}
