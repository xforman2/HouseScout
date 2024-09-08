using Discord;
using Discord.Interactions;
using DiscordBot.Filters;
using Microsoft.EntityFrameworkCore;
using SharedDependencies.Model;
using SharedDependencies.Services;

namespace DiscordBot.Modules
{
    public class RegisterModule : InteractionModuleBase<SocketInteractionContext>
    {
        private static int MinPrice { get; set; }
        private static int MaxPrice { get; set; }
        private static int MinSurface { get; set; }
        private static int MaxSurface { get; set; }
        private static OfferType OfferType { get; set; }
        private static EstateType EstateType { get; set; }

        private readonly IDbContextFactory<HouseScoutContext> _contextFactory;

        public RegisterModule(IDbContextFactory<HouseScoutContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public class EstateModal : IModal
        {
            public string Title => "Estate Details";

            [InputLabel("Min Price")]
            [ModalTextInput("min_price", placeholder: "0", maxLength: 20, initValue: "0")]
            public string MinPriceInput { get; set; }

            [InputLabel("Max Price")]
            [ModalTextInput(
                "max_price",
                placeholder: "99999999",
                maxLength: 20,
                initValue: "20000"
            )]
            public string MaxPriceInput { get; set; }

            [InputLabel("Min Surface Area (m ^ 2)")]
            [ModalTextInput("min_surface", placeholder: "0", maxLength: 20, initValue: "0")]
            public string MinSurfaceInput { get; set; }

            [InputLabel("Max Surface Area (m ^ 2)")]
            [ModalTextInput("max_surface", placeholder: "999", maxLength: 20, initValue: "60")]
            public string MaxSurfaceInput { get; set; }
        }

        [SlashCommand("register", "Register your preferences to receive notifications")]
        public async Task Register()
        {
            await using (var context = await _contextFactory.CreateDbContextAsync())
            {
                var user = context.Users.FirstOrDefault(u => (ulong)u.UserId == Context.User.Id);
                if (user is null)
                {
                    await RespondWithModalAsync<EstateModal>("registerModal");
                }
                else
                {
                    await RespondAsync("User already registered");
                }
            }
        }

        [ModalInteraction("registerModal")]
        public async Task HandleEstateDetailsModalSubmit(EstateModal modal)
        {
            MinPrice = int.Parse(modal.MinPriceInput);
            MaxPrice = int.Parse(modal.MaxPriceInput);
            MinSurface = int.Parse(modal.MinSurfaceInput);
            MaxSurface = int.Parse(modal.MaxSurfaceInput);

            var estateSelectMenu = new SelectMenuBuilder()
                .WithCustomId("estateType")
                .WithPlaceholder("Select estate type")
                .AddOption("House", "house")
                .AddOption("Apartment", "app");

            var builder = new ComponentBuilder().WithSelectMenu(estateSelectMenu);

            await RespondAsync("Please select an estate type:", components: builder.Build());
        }

        [ComponentInteraction("estateType")]
        public async Task HandleEstateTypeSelect(string[] selectedValues)
        {
            EstateType = selectedValues[0] == "house" ? EstateType.HOUSE : EstateType.APARTMENT;

            var offerSelectMenu = new SelectMenuBuilder()
                .WithCustomId("offerType")
                .WithPlaceholder("Select offer type")
                .AddOption("Sale", "sale")
                .AddOption("Rent", "rent");

            var builder = new ComponentBuilder().WithSelectMenu(offerSelectMenu);

            await RespondAsync("Please select an offer type:", components: builder.Build());
        }

        [ComponentInteraction("offerType")]
        public async Task HandleOfferTypeSelect(string[] selectedValues)
        {
            OfferType = selectedValues[0] == "sale" ? OfferType.SALE : OfferType.RENT;

            await using (var context = await _contextFactory.CreateDbContextAsync())
            {
                context.Users.Add(
                    new User(
                        (long)Context.User.Id,
                        MinPrice,
                        MaxPrice,
                        MinSurface,
                        MaxSurface,
                        EstateType,
                        OfferType,
                        true
                    )
                );
                await context.SaveChangesAsync();
            }

            await RespondAsync(
                $"You have been registered with the following preferences:\n"
                    + $"**Offer Type:** {OfferType}\n"
                    + $"**Price Range:** {MinPrice} - {MaxPrice}\n"
                    + $"**Surface Area Range:** {MinSurface} - {MaxSurface}\n"
                    + $"**Estate Type:** {EstateType}"
            );
        }

        [SlashCommand("unregister", "Unregister your notifications")]
        public async Task Unregister()
        {
            await using (var context = await _contextFactory.CreateDbContextAsync())
            {
                var user = context.Users.FirstOrDefault(u => (ulong)u.UserId == Context.User.Id);
                if (user is null)
                {
                    await RespondAsync("User is not registered yet");
                }
                else
                {
                    context.Users.Remove(user);
                    await context.SaveChangesAsync();
                    await RespondAsync("User unregistered");
                }
            }
        }
    }
}
