using System;
using TMPro;
using UnityEngine;

public class TimeClock : MonoBehaviour
{	
	[Header("Clock settings")]
    [SerializeField] private TextMeshProUGUI _clockText;

	[SerializeField] private string _clockformat = "HH:mm:ss";

	[SerializeField] private float _timeRateSpeed = 1f;

	[Header("Started time")]
	[SerializeField, Range(0, 24)] private int _startedHours = 0;
	[SerializeField, Range(0, 60)] private int _startedMinutes = 0;
	[SerializeField, Range(0, 60)] private int _startedSeconds = 0;

	public TimeSpan GetTime => new(_timeSpan.Hours, _timeSpan.Minutes, _timeSpan.Seconds);

	private TimeSpan _timeSpan;

	private void Start()
	{
		_timeSpan = new(_startedHours, _startedMinutes, _startedSeconds);
	}

	private void Update()
	{
		CalculateTime();
	}

	private void CalculateTime()
	{
		float milliSeconds = Time.deltaTime * 1000f * _timeRateSpeed;

		_timeSpan += new TimeSpan(0,0,0,0, (int)milliSeconds);
		
		DateTime dateTime = DateTime.MinValue.Add(_timeSpan);

		_clockText.text = dateTime.ToString(_clockformat);
	}

	private void OnValidate()
	{
		if (_timeRateSpeed < 0)
			_timeRateSpeed = 0;
	}
}
