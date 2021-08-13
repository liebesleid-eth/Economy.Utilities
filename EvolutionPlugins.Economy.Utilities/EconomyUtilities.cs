using OpenMod.API.Plugins;
using OpenMod.Unturned.Plugins;
using System;

[assembly: PluginMetadata("EvolutionPlugins.Economy.Utilities", DisplayName = "Economy Utilities", Website = "https://discord.gg/5MT2yke")]

namespace EvolutionPlugins.Economy.Utilities
{
    public class EconomyUtilities : OpenModUnturnedPlugin
    {

        public EconomyUtilities(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
