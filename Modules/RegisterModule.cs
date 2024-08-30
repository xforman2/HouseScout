using System.Text;
using Discord;
using Discord.Interactions;
using HouseScout.Filters;
using HouseScout.Model;

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
        public RegisterModule(DataFilter filter)
        {
            _filter = filter;
        }
        public class EstateModal : IModal
        {
            public string Title => "Estate Details";
            
            [InputLabel("Min Price")]
            [ModalTextInput("min_price", placeholder: "0", maxLength: 20)]
            public string MinPriceInput { get; set; }

            [InputLabel("Max Price")]
            [ModalTextInput("max_price", placeholder: "99999999", maxLength: 20)]
            public string MaxPriceInput { get; set; }

            [InputLabel("Min Surface Area (m ^ 2)")]
            [ModalTextInput("min_surface", placeholder: "0", maxLength: 20)]
            public string MinSurfaceInput { get; set; }

            [InputLabel("Max Surface Area (m ^ 2)")]
            [ModalTextInput("max_surface", placeholder: "999", maxLength: 20)]
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

            var builder = new ComponentBuilder()
                .WithSelectMenu(estateSelectMenu);
            
            Console.WriteLine(MinPrice);
            

            await RespondAsync("Please select a estate type:", components: builder.Build());
        }
        
        [ComponentInteraction("estateType")]
        public async Task HandleEstateTypeSelect(string[] selectedValues)
        {
            Console.WriteLine(MinPrice);
            EstateType = selectedValues[0] == "house" ? EstateType.HOUSE : EstateType.APARTMENT;

            var offerSelectMenu = new SelectMenuBuilder()
                .WithCustomId("offerType")
                .WithPlaceholder("Select offer type")
                .AddOption("Sale", "sale")
                .AddOption("Rent", "rent");

            var builder = new ComponentBuilder()
                .WithSelectMenu(offerSelectMenu);

            await RespondAsync("Please select a offer type:", components: builder.Build());
        }

        [ComponentInteraction("offerType" )]
            public async Task HandleOfferTypeSelect(string[] selectedValues)
            {
                Console.WriteLine(MinPrice);
                OfferType = selectedValues[0] == "sale" ? OfferType.SALE : OfferType.RENT;
                var estates = _filter.SurfacePriceFilter( MinPrice, MaxPrice, MinSurface, MaxSurface );
                
                if (!estates.Any())
                {
                    await RespondAsync("No matching estates found.");
                    return;
                }

                var messageBuilder = new StringBuilder();
                foreach (var estate in estates)
                {
                    var estateInfo =  $"**Link:** {estate.Link}\n \n";

                    if (messageBuilder.Length + estateInfo.Length > 2000)
                    {
                        await Context.Channel.SendMessageAsync(messageBuilder.ToString());
                        messageBuilder.Clear();
                    }

                    messageBuilder.Append(estateInfo);
                }
                
                if (messageBuilder.Length > 0)
                {
                    await Context.Channel.SendMessageAsync(messageBuilder.ToString());
                }

                await RespondAsync();
            }
        
    }
}