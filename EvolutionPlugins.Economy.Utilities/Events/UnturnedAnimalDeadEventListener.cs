using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using EvolutionPlugins.Economy.Utilities.Helpers;
using EvolutionPlugins.Economy.Utilities.Patches;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
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
        private readonly IStringLocalizer m_StringLocalizer;

        public UnturnedAnimalDeadEventListener(IEconomyProvider economyProvider, IConfiguration configuration, IStringLocalizer stringLocalizer)
        {
            m_EconomyProvider = economyProvider;
            m_Configuration = configuration;
            m_StringLocalizer = stringLocalizer;
        }

        public Task HandleEventAsync(object? sender, UnturnedAnimalDeadEvent @event)
        {
            if (@event.Animal?.Animal != Patch_DamageTool.s_CurrentDamageAnimalParameters.animal)
            {
                return Task.CompletedTask;
            }

            var _damageAnimalParameters = Patch_DamageTool.s_CurrentDamageAnimalParameters;
            if (_damageAnimalParameters.instigator is not Player player)
            {
                return Task.CompletedTask;
            }

            var id = player.channel.owner.playerID.steamID.ToString();
            const string? type = KnownActorTypes.Player;

            var limb = _damageAnimalParameters.limb;
            var parsedLimb = limb.Parse();

            var payMoney = m_Configuration.GetValue<decimal>($"pay:animal:{parsedLimb}", 0);
            if (!payMoney.IsNearlyZero())
            {
                var reason = m_StringLocalizer["balanceUpdationReason:kill:animal"];
                UniTask.RunOnThreadPool(() => m_EconomyProvider.UpdateBalanceAsync(id, type, payMoney, reason));
            }

            return Task.CompletedTask;
        }
    }
}
