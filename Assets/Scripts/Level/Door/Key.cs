using UnityEngine;

public enum DoorKeyTypes
{
	Hall,
	Storage,
	Office,
	Workshop,
	Kitchen,
}

[RequireComponent(typeof(BoxCollider))] //set it triggered
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
