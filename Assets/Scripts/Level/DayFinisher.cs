using DataPersistance;
using Effects;
using Level.Spawners;
using System.Collections;
using TaskSystem.NoteBook;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Level
{
	public class DayFinisher : MonoBehaviour
	{
		[SerializeField] private string _menuSceneName = "Menu";
		[SerializeField] private TimeClock _timeClock;

		[SerializeField] private DissolveEffect _dissolveEffect;

		private IDataService _dataService = new JsonDataService();

		private WeekDay _currentWeekDay = WeekDay.Monday;
		public const string WeekDayPath = "/WeekDay";

		private void Start()
		{
			_dissolveEffect.gameObject.SetActive(false);
			
			_timeClock.OnGameCompleted += FinishDayWork;

			_timeClock.OnGameCompleted += IncreasePlayerDayProgress;

			LoadDayProgress();
		}

		[ContextMenu("Finish Day")]
		public void FinishDayWork()
		{
			_timeClock.OnGameCompleted -= FinishDayWork;

			_dissolveEffect.gameObject.SetActive(true);

			StartCoroutine(PlayDissolveEffect());
		}

		#region Save & Load Systems

		[ContextMenu("Save & Load/" + nameof(LoadDayProgress))]
		public void LoadDayProgress()
		{
			if (_dataService.LoadData(out WeekDay weekDay, WeekDayPath, true))
				_currentWeekDay = weekDay;

			Debug.Log($"Loaded current week day as {_currentWeekDay}");

			SaveDayProgress();
		}

		[ContextMenu("Save & Load/" + nameof(SaveDayProgress))]
		public void SaveDayProgress()
		{
			_dataService.SaveData(WeekDayPath, _currentWeekDay, true);
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

		#endregion

		private IEnumerator PlayDissolveEffect()
		{
			float dissolveEffectStrength = DissolveEffect.MinDissolveEffectStrength;

			while (dissolveEffectStrength < DissolveEffect.MaxDissolveEffectStrength)
			{
				_dissolveEffect.ChangeEffectStrength(dissolveEffectStrength);

				dissolveEffectStrength += Time.deltaTime;

				yield return null;
			}

			SceneManager.LoadScene(_menuSceneName);
		}
	}
}