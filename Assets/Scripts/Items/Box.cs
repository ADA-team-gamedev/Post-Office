using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]

public class Box : MonoBehaviour, IPickable
{
	[field: SerializeField] public BoxData BoxData { get; private set; }

	public Action OnPickUpItem { get; set; }

	public Action OnDropItem { get; set; }
}

