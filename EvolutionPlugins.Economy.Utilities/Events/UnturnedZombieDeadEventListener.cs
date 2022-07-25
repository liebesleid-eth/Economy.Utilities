using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using EvolutionPlugins.Economy.Utilities.Helpers;
using EvolutionPlugins.Economy.Utilities.Patches;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using OpenMod.API.Eventing;
using OpenMod.Core.Users;
using OpenMod.Extensions.Economy.Abstractions;
using OpenMod.Unturned.Zombies.Events;
using SDG.Unturned;

namespace EvolutionPlugins.Economy.Utilities.Events;

internal class UnturnedZombieDeadEventListener : IEventListener<UnturnedZombieDeadEvent>
{
    private readonly IEconomyProvider m_EconomyProvider;
    private readonly IConfiguration m_Configuration;
    private readonly IStringLocalizer m_StringLocalizer;

    public UnturnedZombieDeadEventListener(IEconomyProvider economyProvider, IConfiguration configuration, IStringLocalizer stringLocalizer)
    {
        m_EconomyProvider = economyProvider;
        m_Configuration = configuration;
        m_StringLocalizer = stringLocalizer;
    }

    public Task HandleEventAsync(object? sender, UnturnedZombieDeadEvent @event)
    {
        if (@event.Zombie?.Zombie != Patch_DamageTool.s_CurrentDamageZombieParameters.zombie)
        {
            return Task.CompletedTask;
        }

        var _damageZombieParameters = Patch_DamageTool.s_CurrentDamageZombieParameters;
        if (_damageZombieParameters.instigator is not Player player)
        {
            return Task.CompletedTask;
        }

        var id = player.channel.owner.playerID.steamID.ToString();
        const string Type = KnownActorTypes.Player;

        var parsedLimb = _damageZombieParameters.limb.Parse();
        var isMega = _damageZombieParameters.zombie.isMega;

        var payMoney = m_Configuration.GetValue<decimal>($"pay:zombie{(isMega ? "Mega" : string.Empty)}:{parsedLimb}", 0);
        if (!payMoney.IsNearlyZero())
        {
            var reason = m_StringLocalizer["balanceUpdationReason:kill:zombie"];
            UniTask.RunOnThreadPool(() => m_EconomyProvider.UpdateBalanceAsync(id, Type, payMoney, reason));
        }
        return Task.CompletedTask;
    }
}
