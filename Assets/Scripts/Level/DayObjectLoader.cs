using AYellowpaper.SerializedCollections;
using DataPersistance;
using Level.Map;
using TaskSystem.NoteBook;
using UnityEngine;

namespace Level
{
    public enum WeekDay
    {
        Monday = 1,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday,
    }

    public class DayObjectLoader : MonoBehaviour
    {
        [SerializeField] private TimeClock _timeclock;

        [Header("Save System")]

        private IDataService _dataService = new JsonDataService();

        [field: Header("Spawn Objects")]

        [SerializedDictionary(nameof(WeekDay), nameof(Room))]
        public SerializedDictionary<WeekDay, Room> DayObjects;
        
        private WeekDay _currentWeekDay = WeekDay.Monday;
        public const string WeekDayPath = "/WeekDay";

        private void Start()
        {
            _timeclock.OnGameCompleted += IncreasePlayerDayProgress;

			LoadDayProgress();

            LoadDayObjectsOnMap();
        }

        #region Save System Commands

        [ContextMenu(nameof(LoadDayProgress))]
        public void LoadDayProgress()
        {
            if (_dataService.LoadData(out WeekDay weekDay, WeekDayPath, true))
				_currentWeekDay = weekDay;

            Debug.Log($"Loaded current week day as {_currentWeekDay}");

            SaveDayProgress();
        }

        [ContextMenu(nameof(SaveDayProgress))]
        public void SaveDayProgress()
        {
            _dataService.SaveData(WeekDayPath, _currentWeekDay, true);
        }

        [ContextMenu(nameof(ResetSaves))]
        private void ResetSaves()
        {
            _currentWeekDay = WeekDay.Monday;

            SaveDayProgress();
        }

        #endregion

        private void LoadDayObjectsOnMap()
        {
            for (WeekDay weekDayIndex = WeekDay.Monday; (int)weekDayIndex <= DayObjects.Keys.Count; weekDayIndex++)
            {
                bool isNeedToEnable = weekDayIndex == _currentWeekDay;

                foreach (var item in DayObjects[weekDayIndex].Objects)
                {
                    item.SetActive(isNeedToEnable);
                }
            }
        }

        private void IncreasePlayerDayProgress()
        {
            _timeclock.OnGameCompleted -= IncreasePlayerDayProgress;

            _currentWeekDay++;

            if (_currentWeekDay == WeekDay.Sunday)
                _currentWeekDay = WeekDay.Monday;

            SaveDayProgress();
        }
    }
}
