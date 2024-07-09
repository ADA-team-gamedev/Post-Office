using Audio;
using Player;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Level.Lights.Lamps
{
	public class BreakableLamp : FlickeringLamp, IEvent, IHighlightable
	{
		public bool IsHighlightable => IsLampDestroyed;

		public bool IsLampDestroyed { get; private set; } = false;

		[Header("Destroying Lamp Event")]

		[SerializeField] private ParticleSystem _electronicalSparkVF;

		[Header("Lamp Life Time Delay")]
		[SerializeField] private float _minLampLifeDelayBeforeBreaking = 40;
		[SerializeField] private float _maxLampLifeDelayBeforeBreaking = 180;

		[Space(10)]
		[SerializeField] private string _lampCrushSoundName = "Lamp Crush";

		private float _lampDelayBeforeBreakingRemaining;

		public event Action OnLampDestroyed;
		public event Action OnLampFixed;

		protected override void Awake()
		{
			InitializeLamp();
		}

		protected override void Update()
		{
			TryStartFlashingEvent();

			TryBreakLamp();
		}

		protected virtual void OnTriggerEnter(Collider other)
		{
			TryInvokeLamp(other);
		}

		protected override void InitializeLamp()
		{
			base.InitializeLamp();

			_electronicalSparkVF.gameObject.SetActive(false);

			_lampDelayBeforeBreakingRemaining = Random.Range(_minLampLifeDelayBeforeBreaking, _maxLampLifeDelayBeforeBreaking);
		}

		public override void SwitchLampState(bool isEnabled)
		{
			if (IsLampDestroyed)
				return;

			base.SwitchLampState(isEnabled);
		}

		private void TryBreakLamp()
		{
			if (!IsLampDestroyed)
			{
				_lampDelayBeforeBreakingRemaining -= Time.deltaTime;

				if (_lampDelayBeforeBreakingRemaining <= 0)
					BreakLamp();
			}
			else if (_lampDelayBeforeBreakingRemaining <= 0)
			{
				_lampDelayBeforeBreakingRemaining = Random.Range(_minLampLifeDelayBeforeBreaking, _maxLampLifeDelayBeforeBreaking);
			}
		}

		protected override void TryInvokeLamp(Collider other)
		{
			if (IsLampDestroyed)
				return;

			base.TryInvokeLamp(other);
		}

		protected override bool IsCanStartFlashingEvent()
		{
			if (IsLampDestroyed)
				return false;

			return base.IsCanStartFlashingEvent();
		}

		[ContextMenu(nameof(RepairLamp))]
		public void RepairLamp()
		{
			if (!IsLampDestroyed)
				return;

			OnLampFixed?.Invoke();

			IsLampDestroyed = false;

			LampRenderer.gameObject.SetActive(true);

			_electronicalSparkVF.gameObject.SetActive(false);
	
			SwitchLampState(true);

			StartFlashingEvent();				
		}

		[ContextMenu(nameof(BreakLamp))]
		public void BreakLamp()
		{
			if (IsLampDestroyed)
				return;

			AudioManager.Instance.PlaySound(_lampCrushSoundName, transform.position, spatialBlend: 1f);

			SwitchLampState(false);

			OnLampDestroyed?.Invoke();

			IsLampDestroyed = true;

			LampRenderer.gameObject.SetActive(false);

			_electronicalSparkVF.gameObject.SetActive(true);
		}

		public new void PlayEvent()
		{
			BreakLamp();
		}

		protected override void OnValidate()
		{
			base.OnValidate();

			if (_minLampLifeDelayBeforeBreaking < 0)
				_minLampLifeDelayBeforeBreaking = 0;

			if (_maxLampLifeDelayBeforeBreaking < _minLampLifeDelayBeforeBreaking)
				_maxLampLifeDelayBeforeBreaking = _minLampLifeDelayBeforeBreaking;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
		}
	}
}
