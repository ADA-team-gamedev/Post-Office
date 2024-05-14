using Level.Lights.Lamp;
using Player;
using UnityEngine;

namespace Items
{
	public class LampItem : Item, IUsable
	{
		[SerializeField] private Interactor _playerInteractor;
		
		public void Use()
		{
			if (Physics.Raycast(_playerInteractor.PlayerCamera.transform.position, _playerInteractor.PlayerCamera.transform.forward, out RaycastHit hit, _playerInteractor.InteractionDistance))
			{
				Debug.DrawRay(_playerInteractor.PlayerCamera.transform.position, _playerInteractor.PlayerCamera.transform.forward * _playerInteractor.InteractionDistance);

				if (hit.transform.parent && hit.transform.parent.TryGetComponent(out BreakableLamp lamp) && lamp.IsLampDestroyed)
				{
					lamp.RepairLamp();

					PlayerInventory.Instance.DropItem();

					Destroy(gameObject);		
				}			
			}
		}
	}
}