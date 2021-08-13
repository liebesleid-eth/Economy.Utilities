using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using EvolutionPlugins.Economy.Utilities.Helpers;
using Microsoft.Extensions.Configuration;
using OpenMod.API.Eventing;
using OpenMod.API.Users;
using OpenMod.Core.Users;
using OpenMod.Extensions.Economy.Abstractions;
using OpenMod.Extensions.Games.Abstractions.Players;
using OpenMod.Unturned.Players.Life.Events;
using SDG.Unturned;
using Steamworks;

namespace EvolutionPlugins.Economy.Utilities.Events
{
    public class UnturnedPlayerDeathEventListener : IEventListener<UnturnedPlayerDeathEvent>
    {
        private readonly IEconomyProvider m_EconomyProvider;
        private readonly IConfiguration m_Configuration;
        private readonly IUserManager m_UserManager;

        public UnturnedPlayerDeathEventListener(IEconomyProvider economyProvider, IConfiguration configuration, IUserManager userManager)
        {
            m_EconomyProvider = economyProvider;
            m_Configuration = configuration;
            m_UserManager = userManager;
        }

        public Task HandleEventAsync(object? sender, UnturnedPlayerDeathEvent @event)
        {
            UniTask.RunOnThreadPool(async () =>
            {
                if (@event.Player != null)
                {
                    var loseMoney = m_Configuration.GetValue<decimal>($"lose:death:cause:{@event.DeathCause.ToString().ToLower()}", 0);
                    if (!loseMoney.IsNearlyZero())
                    {
                        loseMoney *= -1.0m;

                        var id = @event.Player.EntityInstanceId;
                        var type = KnownActorTypes.Player;

                        try
                        {
                            await m_EconomyProvider.UpdateBalanceAsync(id, type, loseMoney, null);
                        }
                        catch (NotEnoughBalanceException)
                        {
                            await m_EconomyProvider.SetBalanceAsync(id, type, 0m);
                        }
                    }
                }

                if (@event.Instigator == CSteamID.Nil || @event.Instigator == Provider.server || @event.DeathCause is EDeathCause.SUICIDE)
                {
                    return;
                }

                var user = await m_UserManager.FindUserAsync(KnownActorTypes.Player, @event.Instigator.ToString(),
                    UserSearchMode.FindById);

                if (user is null)
                {
                    return;
                }
                if (user is not OfflineUser && user is IPlayerUser playerUser && playerUser.Player.Equals(@event.Player))
                {
                    return;
                }

                var parsedLimb = @event.Limb switch
                {
                    ELimb.LEFT_FOOT or ELimb.LEFT_LEG or ELimb.RIGHT_FOOT or ELimb.RIGHT_LEG => "leg",
                    ELimb.LEFT_HAND or ELimb.LEFT_ARM or ELimb.RIGHT_HAND or ELimb.RIGHT_ARM => "arm",
                    ELimb.LEFT_BACK or ELimb.RIGHT_BACK or ELimb.LEFT_FRONT or ELimb.RIGHT_FRONT or ELimb.SPINE => "torso",
                    ELimb.SKULL => "head",
                    _ => throw new ArgumentOutOfRangeException(nameof(@event.Limb), "Limb is out of range")
                };

                var payMoney = m_Configuration.GetValue<decimal>($"pay:player:{parsedLimb}", 0);
                if (!payMoney.IsNearlyZero())
                {
                    await m_EconomyProvider.UpdateBalanceAsync(user.Id, user.Type, payMoney, null);
                }
            });

            return Task.CompletedTask;
        }
    }
}
