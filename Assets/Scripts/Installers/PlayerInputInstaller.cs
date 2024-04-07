using Zenject;

namespace Installers
{
	public class PlayerInputInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			PlayerInput playerInput = new();

			playerInput.Enable();

			Container.Bind<PlayerInput>().FromInstance(playerInput).AsSingle();
		}
	}
}