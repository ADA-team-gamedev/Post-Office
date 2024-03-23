using Audio;
using System.Collections;
using UnityEngine;

namespace Level.Lights.Lamp
{
	public class FlickeringLamp : Lamp
	{
		[Header("Lamp flashing event")]

		[SerializeField] private bool _isFlashableLamp = true;

		[SerializeField] private float _timeSinceGameStartToStartFlashing = 60f;
		[SerializeField, Range(1, 100)] private int _flashingStartChance = 50;

		[Space(10)]
		[SerializeField] private float _minFlashingCooldownDelay = 30;
		[SerializeField] private float _maxFlashingCooldownDelay = 60;
		private float _flashingCooldownRemaining = 0;

		[Space(10)]
		[SerializeField] private float _minFlashingDelay = 1;
		[SerializeField] private float _maxFlashingDelay = 7;

		[Space(10)]
		[SerializeField] private FlashingLightCurvesData _flashingCurves;

		private bool _isFlashing = false;

		private float _maxLightRange;
		private float _maxLightIntensity;

		private int _possibleCountOfCurves;

		private void Awake()
		{
			InitializeLamp();
		}

		protected override void InitializeLamp()
		{
			base.InitializeLamp();

			_maxLightIntensity = Light.intensity;

			_maxLightRange = Light.range;

			_possibleCountOfCurves = _flashingCurves.Curves.Count;
		}

		private void Update()
		{
			TryStartFlashingEvent();
		}	

		protected void TryStartFlashingEvent()
		{
			if (_isFlashing || !IsCanStartFlashingEvent())
				return;

			if (_flashingCooldownRemaining <= 0)
			{
				int randomNumber = Random.Range(1, 100);

				if (_flashingStartChance > randomNumber)
				{
					StartFlashingEvent();
				}
				else
				{
					_flashingCooldownRemaining = Random.Range(_minFlashingCooldownDelay, _maxFlashingCooldownDelay);
				}
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

		public void StartFlashingEvent()
		{
			if (_isFlashing)
				return;

			_isFlashing = true;

			AudioManager.Instance.PlaySound("Fuse Flashing", transform.position, spatialBlend: 1);

			float flashingDelay = Random.Range(_minFlashingDelay, _maxFlashingDelay);

			int randomCurveIndex = Random.Range(0, _possibleCountOfCurves);

			_possibleCountOfCurves--;

			if (_possibleCountOfCurves <= 0)
				_possibleCountOfCurves = _flashingCurves.Curves.Count;

			AnimationCurve randomCurve = new(_flashingCurves.Curves[randomCurveIndex].keys);

			AnimationCurve scaledCurve = ScaleCurveToMatchDuration(randomCurve, flashingDelay);

			StartCoroutine(LaunchFlashingAnimation(scaledCurve, flashingDelay));
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

			_isFlashing = false;
		}

		private void OnValidate()
		{
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
