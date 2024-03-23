using System;
using System.Collections;
using TaskSystem;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Player
{
	[RequireComponent(typeof(PlayerDeathController))]
	public class PlayerSanity : MonoBehaviour
	{
		#region Sanity

		[Header("Sanity")]
		[SerializeField][Range(1, 100)] private float _maxSanityValue = 100;

		[SerializeField][Range(0.01f, 10)] private float _sanityDecreaseSpeed = 1f;

		/// <summary>
		/// Return percent from 0.01 to 1
		/// </summary>
		public float SanityPercent => (float)Math.Round(_sanityValue / _maxSanityValue, 2);

		public float Sanity
		{
			get
			{
				return _sanityValue;
			}
			set
			{
				if (value > 0 && value <= _maxSanityValue)
					_sanityValue = value;
				else if (value > _maxSanityValue)
					_sanityValue = _maxSanityValue;
				else
					_sanityValue = 0;
			}
		}
		private float _sanityValue;

		#endregion

		#region Visual

		[Header("Visualization")]

		[SerializeField] private Volume _sanityVolume;

		private float percent;

		[SerializeField] private Slider _slider;

		#endregion

		private PlayerDeathController _playerDeathController;

		private void Start()
		{
			_sanityValue = _maxSanityValue;

			_sanityVolume.weight = 0;

			_slider.maxValue = _maxSanityValue;

			_slider.value = _sanityValue;

			_playerDeathController = GetComponent<PlayerDeathController>();

			_playerDeathController.OnDeath += DisableSanity;

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
				int taskAmount = TaskManager.Instance.TaskCount + 1; // + 1 because we must * it with our sanity. if we have 0 task, means default sanity decrease speed
				
				Sanity -= Time.deltaTime * _sanityDecreaseSpeed * taskAmount;

				float newValue = -(_sanityValue - _maxSanityValue);

				percent = newValue / _maxSanityValue;

				_sanityVolume.weight = percent;

				_slider.value = Sanity;

				yield return null;
			}
		}

		private void DisableSanity()
		{
			Destroy(this);
		}
	}
}