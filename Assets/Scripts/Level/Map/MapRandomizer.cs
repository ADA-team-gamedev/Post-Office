using System;
using UnityEngine;

namespace MapRandomizer
{
	public class MapRandomizer : MonoBehaviour
	{
		[SerializeField] private RoomOption[] _rooms;
	}

	[Serializable]
	internal struct RoomOption
	{
		public Room[] RoomOptions;
	}

	[Serializable]
	internal struct Room
	{
		public GameObject[] Objects;
	}
}
