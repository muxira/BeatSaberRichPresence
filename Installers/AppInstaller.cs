using DiscordRichPresence.Services;
using Zenject;

namespace DiscordRichPresence.Installers
{
    public class AppInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<DiscordPresenceManager>().AsSingle().NonLazy();
        }
    }
}
