using Audio;
using System;
using System.Collections;
using TaskSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Zenject;

namespace Player
{
	[RequireComponent(typeof(PlayerDeathController))]
	public class PlayerSanity : MonoBehaviour
	{
		#region Sanity

		[Header("Sanity")]
		[SerializeField][Range(1, 100)] private float _maxSanityValue = 100;

		[SerializeField][Range(0.01f, 10)] private float _sanityDecreaseSpeed = 1f;

		[SerializeField][Min(5)] private float _playerSanityToStartHeatBeat = 5f;

		[SerializeField] private Animator _playerSanityDeathAnimator;
		[SerializeField] private string _playerSanityDeathTrigger = "Sanity Die";

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
				_sanityValue = Mathf.Clamp(value, 0, _maxSanityValue);
			}
		}
		private float _sanityValue;

		public event Action<float> OnSanityValueChanged;

		private PlayerInput _playerInput;

		#endregion

		#region Visual

		[Header("Visualization")]

		[SerializeField] private Volume _sanityVolume;

		[SerializeField] private TMP_Text _sanityPercentText;

		private float percent;

		[SerializeField] private Slider _slider;

		#endregion

		private PlayerDeathController _playerDeathController;

		private int _taskAmount => TaskManager.Instance.TaskCount + 1; // + 1 because we must * it with our sanity. if we have 0 task, means default sanity decrease speed

		[Inject]
		private void Construct(PlayerInput playerInput)
		{
			_playerInput = playerInput;
		}

		private void Start()
		{
			_sanityValue = _maxSanityValue;

			_sanityVolume.weight = 0;

			_slider.maxValue = _maxSanityValue;

			_slider.value = _sanityValue;

			_playerDeathController = GetComponent<PlayerDeathController>();

			_playerDeathController.OnDied += DisableSanity;

			OnSanityValueChanged += OnWriteSanityPercentText;

			OnSanityValueChanged += OnPlayerNearDeath;

			StartCoroutine(LoseSanity());
		}

		public void IncreaseSanity(float value)
		{
			if (_sanityDecreaseSpeed * _taskAmount >= value)
				Debug.LogWarning("Sanity increase value simillar or less then sanity decreas speed");

			Sanity += Time.deltaTime * value;
		}

		private IEnumerator LoseSanity()
		{
			while (true)
			{
				Sanity -= Time.deltaTime * _sanityDecreaseSpeed * _taskAmount;

				float newValue = -(_sanityValue - _maxSanityValue);

				percent = newValue / _maxSanityValue;

				_sanityVolume.weight = percent;

				_slider.value = Sanity;

				OnSanityValueChanged?.Invoke(Sanity);		

				yield return null;
			}
		}

		private void OnWriteSanityPercentText(float sanityValue)
		{
			_sanityPercentText.text = $"{Mathf.RoundToInt(sanityValue / _maxSanityValue * 100)}%";
		}

		private void OnPlayerNearDeath(float sanityValue)
		{
			if (sanityValue > _playerSanityToStartHeatBeat)
				return;

			if (sanityValue > 0)
			{
				AudioManager.Instance.PlaySound("HeatBeat");
			}
			else
			{
				_playerInput.Player.Disable();

				_playerSanityDeathAnimator.SetTrigger(_playerSanityDeathTrigger);

				AudioManager.Instance.PlaySound("Final HeartBeat");

				_playerDeathController.Die();
			}		
		}

		private void DisableSanity()
		{
			Destroy(this);
		}

		private void OnDestroy()
		{
			_playerDeathController.OnDied -= DisableSanity;

			OnSanityValueChanged -= OnWriteSanityPercentText;

			OnSanityValueChanged -= OnPlayerNearDeath;
		}
	}
}