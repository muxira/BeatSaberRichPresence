using DiscordRichPresence.Services;
using Zenject;

namespace DiscordRichPresence.Installers
{
    public class MenuInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<MenuPresenceService>().AsSingle().NonLazy();
        }
    }
}
