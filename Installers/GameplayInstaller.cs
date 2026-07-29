using DiscordRichPresence.Services;
using Zenject;

namespace DiscordRichPresence.Installers
{
    public class GameplayInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameplayPresenceService>().AsSingle().NonLazy();
        }
    }
}
