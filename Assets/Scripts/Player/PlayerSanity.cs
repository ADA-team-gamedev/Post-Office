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
			return _sanityValue;
		}
		set
		{
			if (value >= 0 && value <= _maxSanityValue)
				_sanityValue = value;
			else if (value > _maxSanityValue)
				_sanityValue = _maxSanityValue;
			else
				_sanityValue = 0;		
		}
	}
	private float _sanityValue;

	[SerializeField] private float _sanityDecreaseSpeed = 1f;

	[SerializeField] private Volume _sanityVolume;

	private float percent;

	#endregion

	private void Start()
	{
		_sanityValue = _maxSanityValue;

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
		
			float newValue = -(_sanityValue - _maxSanityValue);

			percent = newValue / _maxSanityValue;

			_sanityVolume.weight = percent;

			yield return null;
		}
	}
}
