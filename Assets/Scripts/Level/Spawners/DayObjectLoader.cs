using AYellowpaper.SerializedCollections;
using DataPersistance;
using UnityEngine;

namespace Level.Spawners
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
        [field: Header("Spawn Objects")]

        [SerializedDictionary(nameof(WeekDay), nameof(Room))]
        public SerializedDictionary<WeekDay, Room> DayObjects;

		private IDataService _dataService = new JsonDataService();

		private WeekDay _currentWeekDay = WeekDay.Monday;

        private void Start()
        {
			LoadDayProgress();

            LoadDayObjectsOnMap();
        }

		private void LoadDayProgress()
		{
			if (_dataService.TryLoadData(out WeekDay weekDay, JsonDataService.WeekDayPath, true))
				_currentWeekDay = weekDay;
		}

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
    }
}
