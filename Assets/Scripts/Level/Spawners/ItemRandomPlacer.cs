using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Level.Spawners
{
    public class ItemRandomPlacer : MonoBehaviour
    {
        [SerializeField] private PlaceItem[] _itemsToSpawn;

        [SerializeField] private List<Transform> _deffaultPointsToPlace;

		private void Start()
		{
            PlaceItems();
		}

        private void PlaceItems()
        {
            if (_itemsToSpawn.Length <= 0 || _deffaultPointsToPlace.Count <= 0)
            {
#if UNITY_EDITOR
                Debug.Log("No Items or Place for items to spawn!");
#endif
                return;
            }

            for (int i = 0; i < _itemsToSpawn.Length && _deffaultPointsToPlace.Count > 0; i++)
            {
                PlaceItem item = _itemsToSpawn[i];

				if (item.PlaceInSpecificPlaces)
				{
					if (item.SpecificPlacePoints.Count <= 0)
					{
#if UNITY_EDITOR
						Debug.Log("Item must be placed in specific points but item doesn't have them!");
#endif
						continue;
					}

					int randomIndex = Random.Range(0, item.SpecificPlacePoints.Count);

					item.ItemToPlace.position = item.SpecificPlacePoints[randomIndex].position;
				}
				else
				{
					int randomIndex = Random.Range(0, _deffaultPointsToPlace.Count);

					Transform point = _deffaultPointsToPlace[randomIndex];

					item.ItemToPlace.position = point.position;

					_deffaultPointsToPlace.Remove(point);
				}
			}
        }
	}

    [Serializable]
    public class PlaceItem
    {
		[field: SerializeField] public bool PlaceInSpecificPlaces { get; private set; } = true;

        [field: SerializeField] public Transform ItemToPlace { get; private set; }

        [field: SerializeField] public List<Transform> SpecificPlacePoints { get; private set; }
	}
}