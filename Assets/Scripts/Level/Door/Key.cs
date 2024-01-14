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

	public void DropItem()
	{
	
	}

	public void PickUpItem()
	{
		
	}
}
