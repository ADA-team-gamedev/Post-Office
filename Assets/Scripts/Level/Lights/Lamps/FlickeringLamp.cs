using Audio;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Level.Lights.Lamp
{
	public class FlickeringLamp : Lamp, IEvent
	{
		[Header("Lamp flashing event")]

		[SerializeField] private bool _isFlashableLamp = true;

		[SerializeField, Min(0)] private float _timeSinceGameStartToStartFlashing = 60f;
		[SerializeField, Range(1, 100)] private int _flashingStartChance = 50;

		[Space(10)]
		[SerializeField, Min(0)] private float _minFlashingCooldownDelay = 30;
		[SerializeField, Min(0)] private float _maxFlashingCooldownDelay = 60;
		private float _flashingCooldownRemaining = 0;

		[Space(10)]
		[SerializeField, Min(0)] private float _minFlashingDelay = 1;
		[SerializeField, Min(0)] private float _maxFlashingDelay = 7;

		[Space(10)]
		[SerializeField] private FlashingLightCurvesData _flashingCurves;

		[Space(10)]
		[SerializeField] private string _lampFlashingSoundName = "Lamp Flashing";

		private bool _isFlashing = false;

		private float _maxLightRange;
		private float _maxLightIntensity;

		private int _possibleCountOfCurves;

		public event Action OnLampStartFlashing;
		public event Action OnLampStopFlashing;

		protected override void Awake()
		{
			InitializeLamp();
		}

		protected virtual void Update()
		{
			TryStartFlashingEvent();
		}

		protected override void InitializeLamp()
		{
			base.InitializeLamp();

			_maxLightIntensity = Light.intensity;

			_maxLightRange = Light.range;

			_possibleCountOfCurves = _flashingCurves.Curves.Count;
		}

		protected void TryStartFlashingEvent()
		{
			if (_isFlashing || !IsCanStartFlashingEvent())
				return;

			if (_flashingCooldownRemaining <= 0)
			{
				int randomNumber = Random.Range(1, 100);

				if (_flashingStartChance > randomNumber)
					StartFlashingEvent();		
				else
					_flashingCooldownRemaining = Random.Range(_minFlashingCooldownDelay, _maxFlashingCooldownDelay);				
			}
			else
			{
				_flashingCooldownRemaining -= Time.deltaTime;

				_flashingCooldownRemaining = Mathf.Clamp(_flashingCooldownRemaining, 0, _maxFlashingCooldownDelay);
			}
		}

		protected override void TryInvokeLamp(Collider other)
		{
			if (_isFlashing)
				return;

			base.TryInvokeLamp(other);
		}	

		[ContextMenu(nameof(StartFlashingEvent))]
		public void StartFlashingEvent()
		{
			if (_isFlashing)
				return;

			_isFlashing = true;

			OnLampStartFlashing?.Invoke();

			float flashingDelay = Random.Range(_minFlashingDelay, _maxFlashingDelay);

			AudioManager.Instance.PlaySound(_lampFlashingSoundName, transform.position, soundDelay: flashingDelay, spatialBlend: 1f);

			int randomCurveIndex = Random.Range(0, _possibleCountOfCurves);

			_possibleCountOfCurves--;

			if (_possibleCountOfCurves <= 0)
				_possibleCountOfCurves = _flashingCurves.Curves.Count;

			AnimationCurve randomCurve = new(_flashingCurves.Curves[randomCurveIndex].keys);

			AnimationCurve scaledCurve = ScaleCurveToMatchDuration(randomCurve, flashingDelay);

			StartCoroutine(LaunchFlashingAnimation(scaledCurve, flashingDelay));
		}

		public new void PlayEvent()
		{
			StartFlashingEvent();
		}

		protected virtual bool IsCanStartFlashingEvent()
		{
			if (!_isFlashableLamp || Time.timeSinceLevelLoad < _timeSinceGameStartToStartFlashing)
				return false;

			return true;
		}

		private AnimationCurve ScaleCurveToMatchDuration(AnimationCurve curve, float flashingDelay)
		{
			float scaleFactor = flashingDelay / curve.keys[curve.length - 1].time;

			Keyframe[] scaledKeyFrames = new Keyframe[curve.length];

			for (int i = 0; i < curve.length; i++)
			{
				Keyframe keyframe = curve.keys[i];

				scaledKeyFrames[i] = new Keyframe(keyframe.time * scaleFactor, keyframe.value);
			}

			curve.keys = scaledKeyFrames;

			curve.keys[curve.length - 1].value = _maxLightIntensity;

			return curve;
		}

		private IEnumerator LaunchFlashingAnimation(AnimationCurve curve, float flashingDelay)
		{
			float elapsedTime = 0;

			while (elapsedTime < flashingDelay)
			{
				float t = curve.Evaluate(elapsedTime);

				Color currentEmissionColor = Color.Lerp(DisabledLampColor, DefaultLampColor, t);
				
				MaterialPropertyBlock.SetColor(EmissionColor, currentEmissionColor);

				LampRenderer.SetPropertyBlock(MaterialPropertyBlock);

				Light.intensity = Mathf.Lerp(0, _maxLightIntensity, t);

				Light.range = Mathf.Lerp(0, _maxLightRange, t);

				elapsedTime += Time.deltaTime;

				yield return null;
			}

			Light.intensity = _maxLightIntensity;

			Light.range = _maxLightRange;

			_flashingCooldownRemaining = Random.Range(_minFlashingCooldownDelay, _maxFlashingCooldownDelay);

			LampRenderer.material.SetColor(EmissionColor, DefaultLampColor);

			OnLampStopFlashing?.Invoke();

			_isFlashing = false;
		}

		protected override void OnValidate()
		{
			base.OnValidate();

			if (_minFlashingDelay < 0)
				_minFlashingDelay = 0;

			if (_minFlashingDelay > _maxFlashingDelay)
				_minFlashingDelay = _maxFlashingDelay;

			if (_maxFlashingDelay < _minFlashingDelay)
				_maxFlashingDelay = _minFlashingDelay;

			if (_minFlashingCooldownDelay < 0)
				_minFlashingCooldownDelay = 0;

			if (_maxFlashingCooldownDelay < _minFlashingCooldownDelay)
				_maxFlashingCooldownDelay = _minFlashingCooldownDelay;

			if (_minFlashingCooldownDelay > _maxFlashingCooldownDelay)
				_minFlashingCooldownDelay = _maxFlashingCooldownDelay;
		}
	}
}
