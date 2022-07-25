using System;
using OpenMod.API.Plugins;
using OpenMod.Unturned.Plugins;

[assembly: PluginMetadata("EvolutionPlugins.Economy.Utilities", DisplayName = "Economy Utilities", Website = "https://discord.gg/5MT2yke", Author = "DiFFoZ")]

namespace EvolutionPlugins.Economy.Utilities;
public class EconomyUtilitiesPlugin : OpenModUnturnedPlugin
{
    public EconomyUtilitiesPlugin(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}
