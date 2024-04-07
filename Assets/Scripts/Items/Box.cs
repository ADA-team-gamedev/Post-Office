using UnityEngine;

namespace Items
{
	[RequireComponent(typeof(BoxCollider))]
	public class Box : Item
	{
		[field: Header("Box")]
		[field: SerializeField] public BoxData BoxData { get; private set; }
	}
}