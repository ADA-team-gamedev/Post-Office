using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
	[SerializeField] private PlayerInventory _playerInventory;
	[field: SerializeField] public KeyCode PickupKey { get; set; } = KeyCode.E;
	[field: SerializeField] public KeyCode DropKey { get; set; } = KeyCode.G;

	private void Update()
	{
		//if (Input.GetKeyDown(PickupKey))
		//	_playerInventory.TryPickupObject();

		//if (Input.GetKeyDown(DropKey))
		//	_playerInventory.DropItem();
	}
}
