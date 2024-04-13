using TaskSystem.NoteBook;
using UnityEngine;
using Zenject;

namespace Installers
{
    public class TaskSystemsInstaller : MonoInstaller
    {
        [SerializeField] private Tablet _tablet;

		public override void InstallBindings()
		{
			BindTablet();
		}

		private void BindTablet()
		{
			Container.BindInstance(_tablet).AsSingle();
		}
	}
}