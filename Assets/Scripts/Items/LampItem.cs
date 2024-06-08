using Level.Lights.Lamps;
using Player;
using UnityEngine;
using UnityModification;

namespace Items
{
	public class LampItem : Item, IUsable
	{
		[SerializeField] private Interactor _playerInteractor;

		[SerializeField] private LayerMask _lampLayer;
		
		public void Use(Interactor interactor)
		{
			if (Physics.Raycast(_playerInteractor.PlayerCamera.transform.position, _playerInteractor.PlayerCamera.transform.forward, out RaycastHit hit, _playerInteractor.InteractionDistance, _lampLayer))
			{
				EditorDebug.DrawRay(_playerInteractor.PlayerCamera.transform.position, _playerInteractor.PlayerCamera.transform.forward * _playerInteractor.InteractionDistance);

				if (hit.transform.parent && hit.transform.parent.TryGetComponent(out BreakableLamp lamp) && lamp.IsLampDestroyed)
				{
					lamp.RepairLamp();

					if (interactor.Inventory.TryRemoveItem(this))
						Destroy(gameObject);		
				}			
			}
		}
	}
}