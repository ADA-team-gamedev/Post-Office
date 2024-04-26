using Audio;
using Player;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Events.CrushedPC
{
	[SelectionBase]
	[RequireComponent(typeof(BoxCollider))]
	public class CrushedComputerUnit : MonoBehaviour, IInteractable, IEvent
    {
		[Header("Sounds")]
		[SerializeField] private string _pcErrorSound = "PC Error";

		[SerializeField] private string _startHoldingButton = "Case Button Start Click";
		[SerializeField] private string _endHoldingButton = "Case Button Stop Click";

		#region Event Start

		[Header("Object")]
		[SerializeField] private GameObject _screenError;

		[Header("Delays")]
		[SerializeField, Delayed, Min(0)] private float _minTimeSinceGameStartToStartEvent = 90f;
		[SerializeField, Delayed, Min(0)] private float _maxTimeSinceGameStartToStartEvent = 180f;

		private float _timeSinceGameStartToStartEvent;

		[SerializeField, Range(1, 100)] private int _errorEventStartChance = 50;

		[Space(10)]
		[SerializeField, Delayed, Min(0)] private float _minEventCooldownDelay = 30;
		[SerializeField, Delayed, Min(0)] private float _maxEventCooldownDelay = 60;
		private float _eventCooldownRemaining = 0;

		private bool _isPCCrushed = false;

		#endregion

		public event Action OnPCCrushed;
		public event Action OnPCFixed;

		[Header("Fix Button")]
		[SerializeField, Min(0.1f)] private float _buttonHoldingSpeed = 1f;
		[SerializeField, Min(1f)] private float _holdingTimeToOffPC = 2f;

		private float _buttonHoldingTime = 0;

		private void Start()
		{
			_screenError.SetActive(false);

			_timeSinceGameStartToStartEvent = Random.Range(_minTimeSinceGameStartToStartEvent, _maxTimeSinceGameStartToStartEvent);
		}

		private void Update()
		{
			TryStartEvent();
		}

		private void TryStartEvent()
		{
			if (_isPCCrushed || !IsCanStartEvent())
				return;

			if (_eventCooldownRemaining <= 0)
			{
				int randomNumber = Random.Range(1, 100);

				if (_errorEventStartChance > randomNumber)
					CrushPC();
				else
					_eventCooldownRemaining = Random.Range(_minEventCooldownDelay, _maxEventCooldownDelay);
			}
			else
			{
				_eventCooldownRemaining -= Time.deltaTime;

				_eventCooldownRemaining = Mathf.Clamp(_eventCooldownRemaining, 0, _maxEventCooldownDelay);
			}
		}

		public void PlayEvent()
		{
			CrushPC();
		}

		[ContextMenu(nameof(CrushPC))]
		public void CrushPC()
		{
			if (_isPCCrushed)
				return;

			_isPCCrushed = true;

			OnPCCrushed?.Invoke();

			_screenError.SetActive(true);

			AudioManager.Instance.PlaySound(_pcErrorSound, transform.position, spatialBlend: 1f);
		}

		[ContextMenu(nameof(FixPC))]
		private void FixPC()
		{
			if (!_isPCCrushed)
				return;

			AudioManager.Instance.PlaySound(_endHoldingButton, transform.position, spatialBlend: 1f);

			_isPCCrushed = false;

			OnPCFixed?.Invoke();

			_screenError.SetActive(false);

			_eventCooldownRemaining = Random.Range(_minEventCooldownDelay, _maxEventCooldownDelay);

			StopInteract();
		}

		private bool IsCanStartEvent()
		{
			if (Time.timeSinceLevelLoad < _timeSinceGameStartToStartEvent)
				return false;

			return true;
		}

		#region Interaction

		public void StartInteract()
		{
			if (!_isPCCrushed)
				return;

			AudioManager.Instance.PlaySound(_startHoldingButton, transform.position, spatialBlend: 1f);
		}

		public void UpdateInteract()
		{
			if (!_isPCCrushed)
				return;

			_buttonHoldingTime += Time.deltaTime * _buttonHoldingSpeed;

			if (_buttonHoldingTime >= _holdingTimeToOffPC)
				FixPC();		
		}

		public void StopInteract()
		{
			_buttonHoldingTime = 0;
		}

		#endregion

		private void OnValidate()
		{
			if (_maxTimeSinceGameStartToStartEvent <= _minTimeSinceGameStartToStartEvent)
				_maxTimeSinceGameStartToStartEvent = _minTimeSinceGameStartToStartEvent;

			if (_maxEventCooldownDelay <= _minEventCooldownDelay)
				_maxEventCooldownDelay = _minEventCooldownDelay;
		}
	}
}