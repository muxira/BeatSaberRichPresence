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
                activity.Assets.LargeImage = "default_icon";
                updated = true;
            }
            else if (vcName.Contains("Settings"))
            {
                activity.Details = "В настройках";
                activity.State = "";
                updated = true;
            }
            else if (vcName == "SelectLevelCategoryViewController" || vcName == "LevelSelectionNavigationController")
            {
                activity.Details = "Выбирает трек";
                // We could read selectedLevelCategory here if we had direct access, 
                // but setting a generic state is safer against obfuscation/changes.
                activity.State = "Выбор уровня";
                updated = true;
            }

            if (updated)
            {
                presenceManager.SetActivity(activity, immediate: true);
            }
        }
    }
}
