using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class Lamp : MonoBehaviour
{
	[field:SerializeField] public bool IsLampEnabled { get; set; } = true;

	[SerializeField] private string _playerTag = "Player";

	[SerializeField] private Light _light;

	[Space(10)]
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
	[SerializeField] private float _minFlashingDelay = 1;
	[SerializeField] private float _maxFlashingDelay = 7;

	[Space(10)]
	[SerializeField] private FlashingLightCurvesData _flashingCurves;

	private bool _isFlashing = false;

	[Space(10)]
	[SerializeField] private Renderer _lampRenderer;
	private MaterialPropertyBlock _block;

	private float _maxLightRange;
	private float _maxLightIntensity;

	private int _possibleCountOfCurves;

	#endregion

	#region Destroying Properties

	[SerializeField] private ParticleSystem _electronicalSparkVF;

	public bool IsLampDestroyed { get; private set; } = false;

	#endregion

	private void Start()
	{
		_maxLightIntensity = _light.intensity;
		
		_maxLightRange = _light.range;

		_possibleCountOfCurves = _flashingCurves.Curves.Count;

		_block = new();
	}

	private void Update()
	{
		TryStartFlashingEvent();

		if (Input.GetKeyDown(KeyCode.V))
			DestroyLamp();
	}

	private void OnTriggerStay(Collider other)
	{
		if (!IsLampEnabled || IsLampDestroyed || !other.CompareTag(_playerTag))
			return;
	
		OnStay.Invoke();
	}

	public void SwitchLampState(bool isEnabled)
	{
		IsLampEnabled = isEnabled;

		_light.gameObject.SetActive(IsLampEnabled);
	}

	#region Flashing Event

	private void TryStartFlashingEvent()
	{
		if (!_isFlashableLamp || IsLampDestroyed || Time.realtimeSinceStartup < _timeSinceGameStartToStartFlashing)
			return;

		if (!_isFlashing)
		{
			if (_flashingCooldownRemaining <= 0)
			{
				int randomNumber = Random.Range(1, 101);

				if (_flashingStartChance >= randomNumber)
					StartFlashEvent();
			}
			else
			{
				_flashingCooldownRemaining -= Time.deltaTime;

				_flashingCooldownRemaining = Mathf.Clamp(_flashingCooldownRemaining, 0, _maxFlashingCooldownDelay);
			}
		}
	}
	
	public void StartFlashEvent()
	{
		if (_isFlashing)
			return;

		_isFlashing = true;

		float flashingDelay = Random.Range(_minFlashingDelay, _maxFlashingDelay);

		int randomCurveIndex = Random.Range(0, _possibleCountOfCurves);

		_possibleCountOfCurves--;

		if (_possibleCountOfCurves <= 0)
			_possibleCountOfCurves = _flashingCurves.Curves.Count;

		AnimationCurve randomCurve = new(_flashingCurves.Curves[randomCurveIndex].keys);	

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

			Color currentEmissionColor = Color.Lerp(Color.black, Color.white, t);

			_block.SetColor("_EmissionColor", currentEmissionColor);
			_lampRenderer.SetPropertyBlock(_block);

			_light.intensity = Mathf.Lerp(0, _maxLightIntensity, t);

			_light.range = Mathf.Lerp(0, _maxLightRange, t);

			elapsedTime += Time.deltaTime;

			yield return null;
		}

		_light.intensity = _maxLightIntensity;

		_light.range = _maxLightRange;

		_flashingCooldownRemaining = Random.Range(_minFlashingCooldownDelay, _maxFlashingCooldownDelay);

		_lampRenderer.material.SetColor("_EmissionColor", Color.white);

		_isFlashing = false;
	}

	#endregion

	#region Destroying Lamp Event

	public void RepairLamp()
	{
		if (!IsLampDestroyed)
			return;		

		_lampRenderer.gameObject.SetActive(true);

		_light.gameObject.SetActive(true);

		StartFlashEvent();

		IsLampDestroyed = false;
	}

	public void DestroyLamp()
	{
		//if (_isLampDestroyed)
		//	return;

		IsLampDestroyed = true;

		_lampRenderer.gameObject.SetActive(false);

		_light.gameObject.SetActive(false);

		StartCoroutine(PlaySpark(2));
	}

	private IEnumerator PlaySpark(float delay)
	{
		_electronicalSparkVF.Play();

		yield return new WaitForSeconds(delay);

		_electronicalSparkVF.Stop();
	}

	#endregion

	private void OnValidate()
	{
		_light.gameObject?.SetActive(IsLampEnabled);

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
