using UnityEngine;

public enum DoorKeyTypes
{
	Hall,
	Storage,
	Office,
	Workshop,
	Fuse,
	Kitchen,
}

[RequireComponent(typeof(BoxCollider))]

public class Key : Item
{
	[field: SerializeField] public DoorKeyTypes DoorKeyType { get; private set; }
}
