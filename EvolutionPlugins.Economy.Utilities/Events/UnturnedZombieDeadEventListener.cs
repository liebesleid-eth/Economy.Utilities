using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using EvolutionPlugins.Economy.Utilities.Helpers;
using EvolutionPlugins.Economy.Utilities.Patches;
using Microsoft.Extensions.Configuration;
using OpenMod.API.Eventing;
using OpenMod.Core.Users;
using OpenMod.Extensions.Economy.Abstractions;
using OpenMod.Unturned.Zombies.Events;
using SDG.Unturned;

namespace EvolutionPlugins.Economy.Utilities.Events
{
    public class UnturnedZombieDeadEventListener : IEventListener<UnturnedZombieDeadEvent>
    {
        private readonly IEconomyProvider m_EconomyProvider;
        private readonly IConfiguration m_Configuration;

        public UnturnedZombieDeadEventListener(IEconomyProvider economyProvider, IConfiguration configuration)
        {
            m_EconomyProvider = economyProvider;
            m_Configuration = configuration;
        }

        public Task HandleEventAsync(object? sender, UnturnedZombieDeadEvent @event)
        {
            if (@event.Zombie?.Zombie != Patch_DamageTool.s_CurrentDamageZombieParameters.zombie)
            {
                return Task.CompletedTask;
            }

            var _damageZombieParameters = Patch_DamageTool.s_CurrentDamageZombieParameters;
            UniTask.RunOnThreadPool(async () =>
            {
                if (_damageZombieParameters.instigator is not Player player)
                {
                    return;
                }

                var id = player.channel.owner.playerID.steamID.ToString();
                var type = KnownActorTypes.Player;

                var parsedLimb = _damageZombieParameters.limb.Parse();
                var isMega = _damageZombieParameters.zombie.isMega;

                var payMoney = m_Configuration.GetValue<decimal>($"pay:zombie{(isMega ? "Mega" : string.Empty)}:{parsedLimb}", 0);
                if (!payMoney.IsNearlyZero())
                {
                    await m_EconomyProvider.UpdateBalanceAsync(id, type, payMoney, null);
                }
            });

            return Task.CompletedTask;
        }
    }
}
