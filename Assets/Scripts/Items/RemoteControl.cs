using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]

public class RemoteControl : MonoBehaviour, IPickable, IUsable
{
	public Action OnPickUpItem { get; set; }
	public Action OnDropItem { get; set; }

	public void Use()
	{
		if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
		{
			if (hit.transform.parent && hit.transform.parent.TryGetComponent(out GarageDoor garageDoor)) //transform.parent.TryGetComponent() - because garage door script lying on object without collider
				garageDoor.InteractRemotely();
		}
	}
}
