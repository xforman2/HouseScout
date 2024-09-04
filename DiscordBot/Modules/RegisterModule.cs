using System.Text;
using Discord;
using Discord.Interactions;
using HouseScout.Filters;
using HouseScout.Model;
using SharedDependencies.Model;

namespace HouseScout.Modules
{
    public class RegisterModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly DataFilter _filter;
        private static int MinPrice { get; set; }
        private static int MaxPrice { get; set; }
        private static int MinSurface { get; set; }
        private static int MaxSurface { get; set; }
        private static OfferType OfferType { get; set; }
        private static EstateType EstateType { get; set; }

        private static HouseScoutContext _context;

        public RegisterModule(DataFilter filter, HouseScoutContext context)
        {
            _filter = filter;
            _context = context;
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

        [SlashCommand("register", "Register your preferences")]
        public async Task Command()
        {
            await Context.Interaction.RespondWithModalAsync<EstateModal>("registerModal");
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

            await RespondAsync("Please select a estate type:", components: builder.Build());
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

            await RespondAsync("Please select a offer type:", components: builder.Build());
        }

        [ComponentInteraction("offerType")]
        public async Task HandleOfferTypeSelect(string[] selectedValues)
        {
            OfferType = selectedValues[0] == "sale" ? OfferType.SALE : OfferType.RENT;
            _context.Users.Add(
                new User(
                    //Postgres allows only this:  identity column type must be smallint, integer, or bigint
                    (long)Context.User.Id,
                    MinPrice,
                    MaxPrice,
                    MinSurface,
                    MaxSurface,
                    EstateType,
                    OfferType
                )
            );
            await _context.SaveChangesAsync();
            await RespondAsync(
                $"You have been registered with the following preferences:\n"
                    + $"**Offer Type:** {OfferType}\n"
                    + $"**Price Range:** {MinPrice} - {MaxPrice}\n"
                    + $"**Surface Area Range:** {MinSurface} - {MaxSurface}\n"
                    + $"**Estate Type:** {EstateType}"
            );
        }
    }
}
