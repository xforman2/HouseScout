using Discord;
using Discord.Interactions;

namespace HouseScout.Modules;

public class RegisterModule : InteractionModuleBase<SocketInteractionContext>
{
    // Registers a command that will respond with a modal.
    [SlashCommand("register", "Register your preferences")]
    public async Task Command() =>
        await Context.Interaction.RespondWithModalAsync<EstateModal>("register_modal");

    // Defines the modal that will be sent.
    public class EstateModal : IModal
    {
        public string Title => "Estate Details";

        [InputLabel("Price")]
        [ModalTextInput("estate_price", placeholder: "500000", maxLength: 20)]
        public double Price { get; set; }

        [InputLabel("Surface Area ( m ^ 2 )")]
        [ModalTextInput("estate_surface", placeholder: "60", maxLength: 20)]
        public int Surface { get; set; }

        [InputLabel("Estate Type (1=House, 2=Apartment, etc.)")]
        [ModalTextInput("estate_type", placeholder: "1", maxLength: 2)]
        public int EstateType { get; set; }

        [InputLabel("Offer Type (1=Sale, 2=Rent, etc.)")]
        [ModalTextInput("offer_type", placeholder: "1", maxLength: 2)]
        public int OfferType { get; set; }
    }

    [ModalInteraction("register_modal")]
    public async Task ModalResponse(EstateModal modal)
    {
        string message =
            $"New estate listing:\n"
            + $"- Price: ${modal.Price}\n"
            + $"- Surface Area: ${modal.Surface}\n"
            + $"- Estate Type: {modal.EstateType}\n"
            + $"- Offer Type: {modal.OfferType}";

        await RespondAsync(message, ephemeral: true);
    }
}
