using Audio;
using Player;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Modification;

namespace Level.Lights
{
	public class FuseSwitch : DestructiveBehaviour<FuseSwitch>, IInteractable
	{
		[field: SerializeField] public bool IsEnabled { get; private set; } = false;

		[Header("Switch Aniamtion")]

		[SerializeField] private float _newSwitchYPosition = -1f;

		[SerializeField][Range(1f, 100f)] private float _animationSpeed = 15f;

		private float _defaultSwitchYPosition;

		[Header("Actions")]

		public UnityEvent OnSwitchEnabled;
		public UnityEvent OnSwitchDisabled;

		public event Action OnSwitchStateChanged;

		public event Action OnClickedOnSwitch;

		private FuseBox _generator;

		private void Awake()
		{
			_generator = GetComponentInParent<FuseBox>();
		}

		private void Start()
		{
			_defaultSwitchYPosition = transform.position.y;

			if (IsEnabled)
				EnableSwitch();
			else
				DisableSwitch();
		}

		private void Update()
		{
			if (IsEnabled)
				MoveSwitchTo(_defaultSwitchYPosition + _newSwitchYPosition);
			else
				MoveSwitchTo(_defaultSwitchYPosition);
		}

		private void MoveSwitchTo(float newYPosition)
		{
			if (Mathf.Approximately(transform.position.y, newYPosition))
				return;

			Vector3 newPosition = new(transform.position.x, newYPosition, transform.position.z);

			transform.position = Vector3.Lerp(transform.position, newPosition, _animationSpeed * Time.deltaTime);
		}

		#region Player Interaction

		public void StartInteract()
		{
			OnClickedOnSwitch?.Invoke();

			if (IsEnabled)
				DisableSwitch();
			else
				EnableSwitch();

			AudioManager.Instance.PlaySound("Fuse Switch Change", transform.position, spatialBlend: 1);
		}

		public void UpdateInteract() 
		{ 

		}

		public void StopInteract()
		{

		}

		#endregion

		#region Switch Logic

		public void DisableSwitch()
		{
			IsEnabled = false;

			OnSwitchStateChanged?.Invoke();

			OnSwitchDisabled.Invoke();

			_generator.OnFuseEnabled.RemoveListener(ActiveSwitchLater);
		}

		public void EnableSwitch()
		{
			IsEnabled = true;

			OnSwitchStateChanged?.Invoke();

			if (_generator.IsEnabled)
				OnSwitchEnabled.Invoke();
			else
				_generator.OnFuseEnabled.AddListener(ActiveSwitchLater);
		}

		private void ActiveSwitchLater()
		{
			OnSwitchEnabled.Invoke();
		}

		#endregion

		protected override void OnDestroy()
		{
			base.OnDestroy();

			OnSwitchEnabled.RemoveAllListeners();

			OnSwitchDisabled.RemoveAllListeners();
		}
	}
}
