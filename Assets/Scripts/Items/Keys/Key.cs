using UnityEngine;

namespace Items.Keys
{
	public enum DoorKeyTypes
	{
		Hall,
		Storage,
		Toilet,
		Janitor,
		Entrance,
		Exit,
		Office,
		Boss,
		Workshop,
		Fuse,
		Kitchen,
	}

	[RequireComponent(typeof(BoxCollider))]
	public class Key : Item
	{
		[field: Header("Key")]
		[field: SerializeField] public DoorKeyTypes KeyType { get; private set; }
	}
}
