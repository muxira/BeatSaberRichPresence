using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace DiscordRichPresence
{
    internal class PluginConfig
    {
        public static PluginConfig Instance { get; set; }
        
        // Use a dummy string or long. The ID is a long, so we use long.
        public virtual long AppId { get; set; } = 1234567890123456789;
        
        // Any other configuration you might want to add later
        public virtual bool ShowModifiers { get; set; } = true;
    }
}
