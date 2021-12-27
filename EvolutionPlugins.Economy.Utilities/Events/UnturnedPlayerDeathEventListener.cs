using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using EvolutionPlugins.Economy.Utilities.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
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
        private readonly IStringLocalizer m_StringLocalizer;

        public UnturnedPlayerDeathEventListener(IEconomyProvider economyProvider, IConfiguration configuration, IUserManager userManager,
            IStringLocalizer stringLocalizer)
        {
            m_EconomyProvider = economyProvider;
            m_Configuration = configuration;
            m_UserManager = userManager;
            m_StringLocalizer = stringLocalizer;
        }

        public Task HandleEventAsync(object? sender, UnturnedPlayerDeathEvent @event)
        {
            UniTask.RunOnThreadPool(async () =>
            {
                if (@event.Player != null)
                {
                    var loseMoney = m_Configuration.GetValue<decimal>($"lose:death:causes:{@event.DeathCause.ToString().ToLower()}", 0);
                    if (!loseMoney.IsNearlyZero())
                    {
                        loseMoney *= -1.0m;

                        var id = @event.Player.EntityInstanceId;
                        const string? type = KnownActorTypes.Player;
                        var reason = m_StringLocalizer["balanceUpdationReason:death:player",
                            new { DeathReason = @event.DeathCause.ToString().ToLower() }];

                        try
                        {
                            await m_EconomyProvider.UpdateBalanceAsync(id, type, loseMoney, reason);
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
                if (user is IPlayerUser playerUser && playerUser.Player.Equals(@event.Player))
                {
                    return;
                }

                var parsedLimb = @event.Limb.Parse();

                var payMoney = m_Configuration.GetValue<decimal>($"pay:player:{parsedLimb}", 0);
                if (!payMoney.IsNearlyZero())
                {
                    var reason = m_StringLocalizer["balanceUpdationReason:kill:player", new { Player = user }];
                    await m_EconomyProvider.UpdateBalanceAsync(user.Id, user.Type, payMoney, reason);
                }
            });

            return Task.CompletedTask;
        }
    }
}
