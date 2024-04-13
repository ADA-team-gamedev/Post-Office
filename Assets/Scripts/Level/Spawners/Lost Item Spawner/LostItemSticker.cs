using System;
using UnityEngine;

namespace Level.Spawners.LostItemSpawner
{
	[Serializable]
	public class LostItemSticker : MonoBehaviour
	{
		[field: SerializeField] public Transform Photo { get; private set; }

		[field: SerializeField] public Transform Letter { get; private set; }
	}
}