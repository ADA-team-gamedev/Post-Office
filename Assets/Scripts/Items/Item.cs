using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Item : MonoBehaviour
{
	public Action OnPickUpItem { get; set; }

	public Action OnDropItem { get; set; }
}
