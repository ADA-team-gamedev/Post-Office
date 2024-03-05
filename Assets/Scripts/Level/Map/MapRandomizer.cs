using System;
using UnityEngine;

namespace MapRandomizer
{
	public class MapRandomizer : MonoBehaviour
	{
		[SerializeField] private Room[] _rooms;
	}

	[Serializable]
	internal struct RoomOption
	{
		public GameObject[] Objects;
	}

	[Serializable]
	internal struct Room
	{
		public RoomOption[] RoomOptions;
	}
}
