using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using EvolutionPlugins.Economy.Utilities.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using OpenMod.API.Eventing;
using OpenMod.Core.Users;
using OpenMod.Extensions.Economy.Abstractions;
using OpenMod.Unturned.Players.Life.Events;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using Steamworks;
using System.Drawing;

namespace EvolutionPlugins.Economy.Utilities.Events;

internal class UnturnedPlayerDeathEventListener : IEventListener<UnturnedPlayerDeathEvent>
{
    private readonly IEconomyProvider m_EconomyProvider;
    private readonly IConfiguration m_Configuration;
    private readonly IUnturnedUserDirectory m_UnturnedUserDirectory;
    private readonly IStringLocalizer m_StringLocalizer;

    public UnturnedPlayerDeathEventListener(IEconomyProvider economyProvider, IConfiguration configuration, IUnturnedUserDirectory unturnedUserDirectory,
        IStringLocalizer stringLocalizer)
    {
        m_EconomyProvider = economyProvider;
        m_Configuration = configuration;
        m_UnturnedUserDirectory = unturnedUserDirectory;
        m_StringLocalizer = stringLocalizer;
    }

    public Task HandleEventAsync(object? sender, UnturnedPlayerDeathEvent @event)
    {
        var victimUser = m_UnturnedUserDirectory.FindUser(@event.Player.SteamId);

        UniTask.RunOnThreadPool(async () =>
        {
            if (@event.Player != null)
            {
                var loseMoney = m_Configuration.GetValue<decimal>($"lose:death:causes:{@event.DeathCause.ToString().ToLower()}", 0);
                if (!loseMoney.IsNearlyZero())
                {
                    loseMoney *= -1.0m;

                    var id = @event.Player.EntityInstanceId;
                    const string Type = KnownActorTypes.Player;

                    try
                    {
                        await m_EconomyProvider.UpdateBalanceAsync(id, Type, loseMoney, "");
                    }
                    catch (NotEnoughBalanceException)
                    {
                        await m_EconomyProvider.SetBalanceAsync(id, Type, 0m);
                    }
                }
            }

            if (@event.Instigator == CSteamID.Nil
                || @event.Instigator == Provider.server
                || @event.DeathCause is EDeathCause.SUICIDE
                || @event.Instigator == @event.Player?.SteamId)
            {
                return;
            }

            var user = m_UnturnedUserDirectory.FindUser(@event.Instigator);
            if (user is null)
            {
                return;
            }

            var parsedLimb = @event.Limb.Parse();

            var payMoney = m_Configuration.GetValue<decimal>($"pay:player:{parsedLimb}", 0);
            if (!payMoney.IsNearlyZero())
            {
                var reason = m_StringLocalizer["balanceUpdationReason:kill:player", new { Player = user }];
                await m_EconomyProvider.UpdateBalanceAsync(user.Id, user.Type, payMoney, reason);
                await user.PrintMessageAsync(m_StringLocalizer["balanceUpdationReason:kill:playerMessage", new { Player = victimUser }], ColorTranslator.FromHtml(m_Configuration["color"]));
            }
        }).Forget();

        return Task.CompletedTask;
    }
}
