using System;
using UnityEngine;

public enum DoorKeyTypes
{
	Hall,
	Storage,
	Office,
	Workshop,
	Kitchen,
}

public class Key : MonoBehaviour, IPickable
{
	[field: SerializeField] public DoorKeyTypes DoorKeyType { get; private set; }

	public Action OnPickUpItem { get; set; }

	public Action OnDropItem { get; set; }
}
