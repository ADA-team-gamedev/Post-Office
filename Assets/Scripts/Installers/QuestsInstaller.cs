using Zenject;
using UnityEngine;
using Level.Spawners.LostItemSpawner;

namespace Installers
{
	public class QuestsInstaller : MonoInstaller
	{
		[SerializeField] private LostItemSpawner _lostItemSpawner;

		public override void InstallBindings()
		{
			BindLostItemQuest();
		}

		private void BindLostItemQuest()
		{
			Container.BindInstance(_lostItemSpawner).AsSingle();
		}
	}
}