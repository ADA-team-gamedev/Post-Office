using System;
using TMPro;
using UnityEngine;

namespace TaskSystem.NoteBook
{
	public class TimeClock : DestructiveBehaviour<TimeClock>
	{
		[Header("Clock settings")]
		[SerializeField] private TextMeshProUGUI _clockText;

		[SerializeField] private string _clockFormat = "HH:mm:ss";

		[SerializeField] private float _timeRateSpeed = 1f;

		[Header("Time values")]

		[SerializeField] private SerializedTime _startedTime;

		[SerializeField] private SerializedTime _timeToCompleteGame;
		private TimeSpan _timeToComplete;
		private bool _isGameCompleted = false;

		public event Action OnGameCompleted;

		public TimeSpan GetTime => new(_currentGameTime.Hours, _currentGameTime.Minutes, _currentGameTime.Seconds);

		private TimeSpan _currentGameTime;

		private void Start()
		{
			_timeToComplete = new(1, _timeToCompleteGame.Hours, _timeToCompleteGame.Minutes, _timeToCompleteGame.Seconds); //1 day because we start at p.m. and must survive to the new day a.m.

			_currentGameTime = new(_startedTime.Hours, _startedTime.Minutes, _startedTime.Seconds);
		}

		private void Update()
		{
			IsGameOver();

			CalculateTime();
		}

		private void CalculateTime()
		{
			if (_currentGameTime >= _timeToComplete)
				return;

			float milliSeconds = Time.deltaTime * 1000f * _timeRateSpeed;

			_currentGameTime += new TimeSpan(0, 0, 0, 0, (int)milliSeconds);

			DateTime dateTime = DateTime.MinValue.Add(_currentGameTime);

			_clockText.text = dateTime.ToString(_clockFormat);
		}

		public void IsGameOver()
		{
			if (_currentGameTime < _timeToComplete)
				return;

			CompleteGame();
		}

		[ContextMenu(nameof(CompleteGame))]
		private void CompleteGame()
		{
			if (_isGameCompleted)
				return;

			_isGameCompleted = true;
#if UNITY_EDITOR
			Debug.Log("The game is completed!");
#endif
			OnGameCompleted?.Invoke();
		}

		private void OnValidate()
		{
			if (_timeRateSpeed < 0)
				_timeRateSpeed = 0;
		}
	}

	[Serializable]
	public struct SerializedTime
	{
		[field: SerializeField, Range(0, 24)] public int Hours { get; private set; }
		[field: SerializeField, Range(0, 60)] public int Minutes { get; private set; }
		[field: SerializeField, Range(0, 60)] public int Seconds { get; private set; }
	}
}