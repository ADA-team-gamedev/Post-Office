using UnityEngine;

[RequireComponent(typeof(BoxCollider))]

public class Box : Item
{
	[field: SerializeField] public BoxData BoxData { get; private set; }
}

