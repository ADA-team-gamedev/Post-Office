using Items;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityModification;
using Random = UnityEngine.Random;

namespace Level.Spawners
{
    public class ItemRandomPlacer : MonoBehaviour
    {
        [SerializeField] private PlaceItem[] _itemsToSpawn;

        [SerializeField] private List<Transform> _deffaultPointsToPlace;

		private List<Transform> _usedSpecificPoints = new();

		private void Start()
		{
            PlaceItems();
		}

        private void PlaceItems()
        {
            if (_itemsToSpawn.Length <= 0)
            {
				EditorDebug.LogWarning("No Items to place!");

                return;
            }

			foreach (PlaceItem item in _itemsToSpawn)
			{
				if (item.PlaceInSpecificPlace)
				{
					if (item.SpecificPlacePoints.Count <= 0)
					{
						EditorDebug.LogWarning("Item must be placed in specific points but item doesn't have them!");

						continue;
					}

					PlaceItemInSpecificPlace(item);
				}
				else
				{
					if (_deffaultPointsToPlace.Count <= 0)
					{
						EditorDebug.LogWarning("No deffault points to place!");

						continue;
					}

					PlaceItemInDeffaultPlace(item);
				}

				if (item.RotateRandomly)
				{
					Vector3 deffaultEulerRotation = item.ItemToPlace.gameObject.transform.rotation.eulerAngles;

					float newYAngle = Random.Range(0, 360);

					item.ItemToPlace.gameObject.transform.rotation = Quaternion.Euler(deffaultEulerRotation.x, newYAngle, deffaultEulerRotation.z);
				}

				item.ItemToPlace.ItemIcon.ShowIcon(item.ItemToPlace);
			}
        }

		private void PlaceItemInSpecificPlace(PlaceItem item)
		{
			for (int i = 0; i < item.SpecificPlacePoints.Count; i++)
			{
				int randomIndex = Random.Range(0, item.SpecificPlacePoints.Count);
				
				Transform point = item.SpecificPlacePoints[randomIndex];
				
				if (_usedSpecificPoints.Contains(point))
				{
					EditorDebug.LogWarning($"{item} has specific point which we already used to spawn another one!");

					item.RemoveSpecificPoint(point);

					continue;
				}

				item.ItemToPlace.transform.position = point.position;

				_usedSpecificPoints.Add(point);

				break;
			}
		}

		private void PlaceItemInDeffaultPlace(PlaceItem item)
		{
			for (int i = 0; i < _deffaultPointsToPlace.Count; i++)
			{
				int randomIndex = Random.Range(0, _deffaultPointsToPlace.Count);

				Transform point = _deffaultPointsToPlace[randomIndex];
				
				if (_usedSpecificPoints.Contains(point))
				{
					EditorDebug.LogWarning($"{item} want to be place on specific point({point}) which we already used to spawn another one!");

					_deffaultPointsToPlace.Remove(point);

					continue;
				}
				
				item.ItemToPlace.transform.position = point.position;

				_deffaultPointsToPlace.Remove(point);
				
				break;
			}
		}
	}

    [Serializable]
    public class PlaceItem
    {
		[field: SerializeField] public bool PlaceInSpecificPlace { get; private set; } = true;

		[field: SerializeField] public bool RotateRandomly { get; private set; } = true;

        [field: SerializeField] public Item ItemToPlace { get; private set; }

		[SerializeField] private List<Transform> _specificPlacePoints;

		public IReadOnlyList<Transform> SpecificPlacePoints => _specificPlacePoints;

		public void RemoveSpecificPoint(Transform point)
			=> _specificPlacePoints.Remove(point);
	}
}