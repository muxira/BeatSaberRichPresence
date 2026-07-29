using System;
using System.Linq;
using UnityEngine;
using Zenject;
using Discord;
using HMUI; // From HMUI.dll

namespace DiscordRichPresence.Services
{
    public class MenuPresenceService : IInitializable, IDisposable
    {
        private readonly DiscordPresenceManager _presenceManager;

        // Try to inject flow coordinators if available in context, otherwise we will find them
        [InjectOptional] private readonly MainFlowCoordinator _mainFlowCoordinator;

        public MenuPresenceService(DiscordPresenceManager presenceManager)
        {
            _presenceManager = presenceManager;
        }

        public void Initialize()
        {
            UpdateMenuPresence("In Main Menu");

            if (_mainFlowCoordinator != null)
            {
                // This is a bit brittle, but standard for Beat Saber
                // We would normally hook into didActivateEvent of ViewControllers
            }

            // We can also poll or use a simple Harmony patch to track ViewControllers, 
            // but for safety and simplicity let just set a generic menu presence if we cant hook specific ones.
            // A more robust way is hooking HMUI.FlowCoordinator.didActivateEvent using Harmony.
        }

        public void Dispose()
        {
        }

        private void UpdateMenuPresence(string details, string state = "")
        {
            var activity = new Activity
            {
                Details = details,
                State = state,
                Assets = new ActivityAssets
                {
                    LargeImage = "default_icon",
                    LargeText = "Menu"
                }
            };
            
            _presenceManager.SetActivity(activity, immediate: true);
        }
    }
}
