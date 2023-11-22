using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerSanity : MonoBehaviour
{
	#region SanitySettings

	[SerializeField][Range(1, 100f)] private float _maxSanityValue = 100f;

	public float Sanity
	{
		get
		{
			return _sanity;
		}
		set
		{
			if (value >= 0 && value <= _maxSanityValue)
				_sanity = value;
		}
	}

	private float _sanity;

	[SerializeField] private Slider _sanitySlider;
	[SerializeField] private float _sanityDecreaseSpeed = 1f;

	[SerializeField] private Volume _sanityVolume;

	private float percent;

	#endregion

	private void Start()
	{
		_sanity = _maxSanityValue;

		_sanitySlider.maxValue = _maxSanityValue;

		_sanitySlider.value = _sanity;

		_sanityVolume.weight = 0;

		StartCoroutine(LoseSanity());
	}

	public void IncreaseSanity(float value)
	{
		if (_sanityDecreaseSpeed >= value)
			Debug.LogWarning("Sanity increase value simillar or less then sanity decreas speed");
		
		Sanity += Time.deltaTime * value;
	}

	private IEnumerator LoseSanity()
	{
		while (true)
		{
			Sanity -= Time.deltaTime * _sanityDecreaseSpeed;

			_sanitySlider.value = _sanity;
			float newValue = -(_sanitySlider.value - _sanitySlider.maxValue);

			percent = newValue / _maxSanityValue;

			_sanityVolume.weight = percent;

			yield return null;
		}
	}
}
