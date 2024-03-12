using System;
using UnityEngine;

namespace Level.Map
{
	public class MapRandomizer : MonoBehaviour
	{
		[SerializeField] private RoomOption[] _roomOptions;
	}

	[Serializable]
	public struct RoomOption
	{
		public Room[] Rooms;
	}

	[Serializable]
	public struct Room
	{
		public GameObject[] Objects;
	}
}
