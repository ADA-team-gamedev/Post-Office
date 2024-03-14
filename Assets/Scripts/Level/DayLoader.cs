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

    public class DayLoader : MonoBehaviour
    {
        [SerializeField] private TimeClock _timeclock;

        [Header("Save System")]

        private IDataService _dataService = new JsonDataService();

        [Header("Spawn Objects")]

        [SerializedDictionary(nameof(WeekDay), nameof(Room))]
        public SerializedDictionary<WeekDay, Room> _dayObjects;
        
        private WeekDay _currentWeekDay = WeekDay.Monday;
        private const string _path = "/WeekDay";

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
            if (_dataService.LoadData(out WeekDay weekDay, _path, true))
				_currentWeekDay = weekDay;

            SaveDayProgress();
        }

        [ContextMenu(nameof(SaveDayProgress))]
        public void SaveDayProgress()
        {
            _dataService.SaveData(_path, _currentWeekDay, true);
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
            for (WeekDay weekDayIndex = WeekDay.Monday; (int)weekDayIndex <= _dayObjects.Keys.Count; weekDayIndex++)
            {
                bool isNeedToEnable = weekDayIndex == _currentWeekDay;

                foreach (var item in _dayObjects[weekDayIndex].Objects)
                {
                    item.SetActive(isNeedToEnable);
                }
            }
        }

        private void IncreasePlayerDayProgress()
        {
            _currentWeekDay++;

            SaveDayProgress();
        }

        #region Temp Changers

        [ContextMenu("Increase")]
        private void IncreaseWeekDay()
        {
            _currentWeekDay++;
        }

        [ContextMenu("Decrease")]
        private void DecreaseWeekDay()
        {
            _currentWeekDay--;
        }

        #endregion
    }
}
