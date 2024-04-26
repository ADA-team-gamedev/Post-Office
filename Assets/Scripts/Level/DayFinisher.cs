using Audio;
using DataPersistance;
using Level.Spawners;
using TaskSystem.NoteBook;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Level
{
	public class DayFinisher : MonoBehaviour
	{
		[Header("Effect")]
		[SerializeField] private Animator _dissolveEffect;
		[SerializeField] private string _dissolveEffectTrigger = "Show";
		[SerializeField, Min(1f)] private float _dissolveEffectDelay = 1f;

		[Header("Scene Name")]
		[SerializeField] private string _menuSceneName = "Menu";

		[Header("Time Clock")]
		[SerializeField] private TimeClock _timeClock;

		private IDataService _dataService = new JsonDataService();

		private WeekDay _currentWeekDay = WeekDay.Monday;	

		private void Start()
		{
			_timeClock.OnGameCompleted += FinishDayWork;

			_timeClock.OnGameCompleted += IncreasePlayerDayProgress;

			LoadDayProgress();
		}

		[ContextMenu("Finish Day")]
		public void FinishDayWork()
		{
			_timeClock.OnGameCompleted -= FinishDayWork;

			AudioManager.Instance.PlaySound("Click", transform.position);

			_dissolveEffect.SetTrigger(_dissolveEffectTrigger);

			Invoke(nameof(LoadMenu), _dissolveEffectDelay);
		}

		#region Save & Load Systems

		[ContextMenu("Save & Load/" + nameof(LoadDayProgress))]
		public void LoadDayProgress()
		{
			if (_dataService.TryLoadData(out WeekDay weekDay, JsonDataService.WeekDayPath, true))
				_currentWeekDay = weekDay;

#if UNITY_EDITOR
			Debug.Log($"Loaded current week day as {_currentWeekDay}");
#endif

			SaveDayProgress();
		}

		[ContextMenu("Save & Load/" + nameof(SaveDayProgress))]
		public void SaveDayProgress()
		{
			_dataService.SaveData(JsonDataService.WeekDayPath, _currentWeekDay, true);
		}

		[ContextMenu("Save & Load/" + nameof(ResetSaves))]
		public void ResetSaves()
		{
			_currentWeekDay = WeekDay.Monday;

			SaveDayProgress();
		}

		private void IncreasePlayerDayProgress()
		{
			_timeClock.OnGameCompleted -= IncreasePlayerDayProgress;

			_currentWeekDay++;

			if (_currentWeekDay == WeekDay.Sunday)
				_currentWeekDay = WeekDay.Monday;

			SaveDayProgress();
		}

		private void LoadMenu()
		{
			if (_dataService.SaveData(JsonDataService.LoadingInfoPath, _menuSceneName, true))
				SceneManager.LoadScene(SceneLoader.LoadingSceneName);
		}

		#endregion

		private void OnDestroy()
		{
			_timeClock.OnGameCompleted -= FinishDayWork;

			_timeClock.OnGameCompleted -= IncreasePlayerDayProgress;
		}
	}
}