using IPA;
using IPA.Config;
using IPA.Config.Stores;
using SiraUtil.Zenject;
using System;
using IPALogger = IPA.Logging.Logger;
using DiscordRichPresence.Installers;
using HarmonyLib;

namespace DiscordRichPresence
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        public static IPALogger Log { get; private set; } = null!;
        public const string HarmonyId = "com.antigravity.BeatSaber.DiscordRichPresence";
        private Harmony _harmony = null!;

        [Init]
        public void Init(IPALogger logger, Zenjector zenjector)
        {
            Log = logger;
            _harmony = new Harmony(HarmonyId);
            
            // Set up our Zenject installers
            zenjector.Install<AppInstaller>(Location.App);
            zenjector.Install<MenuInstaller>(Location.Menu);
            zenjector.Install<GameplayInstaller>(Location.StandardPlayer);
            
            Log.Info("DiscordRichPresence initialized.");
        }

        [OnEnable]
        public void OnEnable()
        {
            _harmony.PatchAll(System.Reflection.Assembly.GetExecutingAssembly());
        }

        [OnDisable]
        public void OnDisable()
        {
            _harmony.UnpatchSelf();
        }
    }
}
