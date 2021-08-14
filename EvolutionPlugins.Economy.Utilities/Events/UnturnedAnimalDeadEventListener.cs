using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using EvolutionPlugins.Economy.Utilities.Helpers;
using EvolutionPlugins.Economy.Utilities.Patches;
using Microsoft.Extensions.Configuration;
using OpenMod.API.Eventing;
using OpenMod.Core.Users;
using OpenMod.Extensions.Economy.Abstractions;
using OpenMod.Unturned.Animals.Events;
using SDG.Unturned;

namespace EvolutionPlugins.Economy.Utilities.Events
{
    public class UnturnedAnimalDeadEventListener : IEventListener<UnturnedAnimalDeadEvent>
    {
        private readonly IEconomyProvider m_EconomyProvider;
        private readonly IConfiguration m_Configuration;

        public UnturnedAnimalDeadEventListener(IEconomyProvider economyProvider, IConfiguration configuration)
        {
            m_EconomyProvider = economyProvider;
            m_Configuration = configuration;
        }

        public Task HandleEventAsync(object? sender, UnturnedAnimalDeadEvent @event)
        {
            if (@event.Animal?.Animal != Patch_DamageTool.s_CurrentDamageAnimalParameters.animal)
            {
                return Task.CompletedTask;
            }

            var _damageAnimalParameters = Patch_DamageTool.s_CurrentDamageAnimalParameters;
            var _params = Patch_DamageAnimalParameters.s_CurrentDamageAnimalParameters;
            UniTask.RunOnThreadPool(async () =>
            {
                if (_damageAnimalParameters.instigator is not Player player)
                {
                    return;
                }

                var id = player.channel.owner.playerID.steamID.ToString();
                var type = KnownActorTypes.Player;

                var limb = ELimb.SPINE;
                if (_params?.animal != null && _params?.animal == _damageAnimalParameters.animal)
                {
                    limb = _params.Value.limb;
                }
                var parsedLimb = limb.Parse();

                var payMoney = m_Configuration.GetValue<decimal>($"pay:animal:{parsedLimb}", 0);
                if (!payMoney.IsNearlyZero())
                {
                    await m_EconomyProvider.UpdateBalanceAsync(id, type, payMoney, null);
                }
            });

            return Task.CompletedTask;
        }
    }
}
