using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using IEnumerator = System.Collections.IEnumerator;

[RequireComponent(typeof(BoxCollider))]
public class Lamp : MonoBehaviour
{
	[field:SerializeField] public bool IsLampEnabled { get; set; } = true;

	[SerializeField] private string _playerTag = "Player";

	[SerializeField] private Light _light;

	[SerializeField] private UnityEvent OnStay;

	#region Flashing properties

	[Header("Lamp flashing event")]

	[SerializeField] private bool _isFlashableLamp = false;

	[SerializeField] private float _timeSinceGameStartToStartFlashing = 60f;
	[SerializeField, Range(1, 100)] private int _flashingStartChance = 10;

	[Space(10)]
	[SerializeField] private float _minFlashingCooldownDelay = 30;
	[SerializeField] private float _maxFlashingCooldownDelay = 60;
	private float _flashingCooldownRemaining = 0;

	[Space(10)]
	[SerializeField] private float _minFlashingDelay = 3;
	[SerializeField] private float _maxFlashingDelay = 10;

	[Space(10), SerializeField] private LightFlashingAnimationCurves _flashingCurves;

	private bool _isFlashing = false;

	private Color _lightEmissionOffColor = Color.black;

	private float _maxLightRange;
	private float _maxLightIntensity;

	#endregion

	private void Start()
	{
		_maxLightIntensity = _light.intensity;

		_maxLightRange = _light.range;
	}

	private void Update()
	{
		TryStartFlashingEvent();
	}

	private void OnTriggerStay(Collider other)
	{
		if (!IsLampEnabled || !other.CompareTag(_playerTag))
			return;
	
		OnStay.Invoke();
	}

	public void SwitchLampState(bool isEnabled)
	{
		IsLampEnabled = isEnabled;

		_light.gameObject.SetActive(IsLampEnabled);
	}

	#region Flashing

	private void TryStartFlashingEvent()
	{
		if (!_isFlashableLamp || Time.realtimeSinceStartup < _timeSinceGameStartToStartFlashing)
			return;

		if (!_isFlashing)
		{
			if (_flashingCooldownRemaining <= 0)
			{
				int randomNumber = Random.Range(1, 101);

				if (_flashingStartChance >= randomNumber)
					EnableFlashing();
			}
			else
			{
				_flashingCooldownRemaining -= Time.deltaTime;

				_flashingCooldownRemaining = Mathf.Clamp(_flashingCooldownRemaining, 0, _maxFlashingCooldownDelay);
			}
		}
	}
	
	public void EnableFlashing()
	{
		if (_isFlashing)
			return;

		_isFlashing = true;

		float flashingDelay = Random.Range(_minFlashingDelay, _maxFlashingDelay);

		int randomCurveIndex = Random.Range(0, _flashingCurves.FlashingCurves.Count);

		AnimationCurve randomCurve = new AnimationCurve(_flashingCurves.FlashingCurves[randomCurveIndex].keys);

		AnimationCurve scaledCurve = ScaleCurveToMatchDuration(randomCurve, flashingDelay);

		StartCoroutine(LaunchFlashingAnimation(scaledCurve, flashingDelay));
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

			//Color curretnEmissionColor = Color.Lerp(Color.black, Color.white, t);

			_light.intensity = Mathf.Lerp(0, _maxLightIntensity, t);

			_light.range = Mathf.Lerp(0, _maxLightRange, t);

			elapsedTime += Time.deltaTime;

			yield return null;
		}

		_light.intensity = _maxLightIntensity;

		_light.range = _maxLightRange;

		_flashingCooldownRemaining = Random.Range(_minFlashingCooldownDelay, _maxFlashingCooldownDelay);

		_isFlashing = false;
	}

	#endregion

	private void OnValidate()
	{
		_light.gameObject?.SetActive(IsLampEnabled);

		if (_minFlashingDelay > _maxFlashingDelay)
			_minFlashingDelay = _maxFlashingDelay;

		if (_maxFlashingDelay < _minFlashingDelay)
			_maxFlashingDelay = _minFlashingDelay;

		if (_maxFlashingCooldownDelay < _minFlashingCooldownDelay)
			_maxFlashingCooldownDelay = _minFlashingCooldownDelay;

		if (_minFlashingCooldownDelay > _maxFlashingCooldownDelay)
			_minFlashingCooldownDelay = _maxFlashingCooldownDelay;
	}
}

[CreateAssetMenu]
public class LightFlashingAnimationCurves : ScriptableObject
{
	[field: SerializeField] public List<AnimationCurve> FlashingCurves { get; private set; }
}
