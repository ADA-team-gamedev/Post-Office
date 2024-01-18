using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoxContentType
{
	Empty,
	Glass,
	Metal,
	Toy,
	Book,
	Electronic,
}

public enum BoxTemperatureType
{
	Fiery,
	Hot,
	Normal,
	Cold,
	Frosty,
}

[CreateAssetMenu]
public class BoxData : ScriptableObject
{
	[field: SerializeField] public BoxContentType ContentType { get; private set; } = BoxContentType.Empty;

	[field: SerializeField] public BoxTemperatureType BoxTemperatureType { get; private set; } = BoxTemperatureType.Normal;
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]

public class Box : MonoBehaviour, IPickable
{
	public BoxData BoxData { get; private set; }

	public Action OnPickUpItem { get; set; }

	public Action OnDropItem { get; set; }
}

