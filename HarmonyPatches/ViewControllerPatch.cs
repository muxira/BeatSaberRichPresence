using HarmonyLib;
using HMUI;
using Zenject;
using DiscordRichPresence.Services;

namespace DiscordRichPresence.HarmonyPatches
{
    // We hook into ViewController DidActivate to update Discord presence for menu navigation.
    // This is less brittle than finding specific FlowCoordinators.
    [HarmonyPatch(typeof(ViewController), "DidActivate")]
    internal class ViewControllerPatch
    {
        private static void Postfix(ViewController __instance, bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (!addedToHierarchy) return;

            string vcName = __instance.GetType().Name;
            
            // Try to get DiscordPresenceManager from ProjectContext
            var container = ProjectContext.Instance?.Container;
            if (container == null) return;
            
            var presenceManager = container.TryResolve<DiscordPresenceManager>();
            if (presenceManager == null) return;

            var activity = presenceManager.CurrentActivity;
            bool updated = false;

            if (vcName == "MainMenuViewController")
            {
                activity.Details = "В главном меню";
                activity.State = "";
                activity.Timestamps = default;
                activity.Assets.LargeImage = "default_icon";
                updated = true;
            }
            else if (vcName.Contains("Settings"))
            {
                activity.Details = "В настройках";
                activity.State = "";
                activity.Timestamps = default;
                updated = true;
            }
            else if (vcName.Contains("LevelCollection") || vcName.Contains("LevelSelection") || vcName.Contains("LevelDetail"))
            {
                activity.Details = "Выбирает трек";
                activity.State = "Выбор уровня";
                activity.Timestamps = default;
                updated = true;
            }

            if (updated)
            {
                presenceManager.SetActivity(activity, immediate: true);
            }
        }
    }
}
