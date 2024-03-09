using UnityEngine;

public class LampItem : Item, IUsable
{
	[SerializeField] private Camera _playerCamera;
	[SerializeField, Range(0.5f, 5f)] private float _interactionDistance = 1f;

	public void Use()
	{
		if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out RaycastHit hit, _interactionDistance))
		{
			Debug.DrawRay(_playerCamera.transform.position, _playerCamera.transform.forward * _interactionDistance);

			if (hit.transform.TryGetComponent(out Lamp lamp) && lamp.IsLampDestroyed)
			{
				lamp.RepairLamp();

				PlayerInventory.Instance.DropItem();

				Destroy(gameObject);
			}
		}
	}
}
