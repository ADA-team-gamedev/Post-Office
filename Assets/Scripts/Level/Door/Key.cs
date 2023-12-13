using System.Collections;
using System.Collections.Generic;
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
public class Key : MonoBehaviour
{
	[field: SerializeField] public DoorKeyTypes DoorKeyType { get; private set; }
}
