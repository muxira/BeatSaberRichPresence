using System;
using UnityEngine;
using Zenject;
using Discord; // From DiscordCore
using DiscordCore; // Added to fix DiscordInstance

namespace DiscordRichPresence.Services
{
    public class DiscordPresenceManager : IInitializable, IDisposable, ITickable
    {
        // Replace with actual Discord app ID
        // Note: The user must create an app on Discord Developer Portal and provide this ID.
        public const long AppId = 1234567890123456789; // TODO: Replace with real AppID

        private DiscordInstance _discordInstance;
        private Activity _currentActivity;
        
        private float _lastUpdateTime = 0f;
        private bool _activityDirty = false;
        private const float UpdateInterval = 1.5f; // Discord rate limit is ~15s per 5 events, 1.5s is safe for batched
        
        public Activity CurrentActivity => _currentActivity;

        public void Initialize()
        {
            Plugin.Log.Info("Initializing DiscordPresenceManager...");
            
            // Create the Discord instance via DiscordCore
            _discordInstance = DiscordManager.instance.CreateInstance(
                new DiscordSettings 
                { 
                    appId = AppId,
                    handleInvites = false,
                    modId = "DiscordRichPresence",
                    modName = "DiscordRichPresence"
                });
                
            _currentActivity = new Activity
            {
                State = "Starting up...",
                Details = "Beat Saber",
                Assets = new ActivityAssets
                {
                    LargeImage = "default_icon" // Make sure to upload this to the dev portal
                }
            };
            
            _activityDirty = true;
        }

        public void Dispose()
        {
            Plugin.Log.Info("Disposing DiscordPresenceManager...");
            if (_discordInstance != null)
            {
                _discordInstance.ClearActivity();
                _discordInstance.DestroyInstance();
                _discordInstance = null;
            }
        }

        public void Tick()
        {
            if (_activityDirty && Time.time - _lastUpdateTime >= UpdateInterval)
            {
                ForceUpdateActivity();
            }
        }

        public void SetActivity(Activity activity, bool immediate = false)
        {
            _currentActivity = activity;
            _activityDirty = true;
            
            if (immediate)
            {
                ForceUpdateActivity();
            }
        }

        public void ForceUpdateActivity()
        {
            if (_discordInstance != null)
            {
                _discordInstance.UpdateActivity(_currentActivity);
                _lastUpdateTime = Time.time;
                _activityDirty = false;
            }
        }
        
        public void ClearActivity()
        {
            if (_discordInstance != null)
            {
                _discordInstance.ClearActivity();
            }
        }
    }
}
