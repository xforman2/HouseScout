using System.Text;
using Discord.WebSocket;
using DiscordBot.Filters;
using Microsoft.EntityFrameworkCore;
using SharedDependencies.Model;
using SharedDependencies.Services;

namespace DiscordBot.Services
{
    public class DiscordMessageHandler : IMessageHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly HouseScoutContext _context;
        private readonly DataFilter _dataFilter;

        public DiscordMessageHandler(
            DiscordSocketClient client,
            HouseScoutContext context,
            DataFilter dataFilter
        )
        {
            _client = client;
            _context = context;
            _dataFilter = dataFilter;
        }

        public async Task HandleMessageAsync()
        {
            var users = await _context.Users.AsNoTracking().ToListAsync();
            foreach (var dbUser in users)
            {
                var user = await _client.GetUserAsync((ulong)dbUser.UserId);
                if (user != null)
                {
                    var dmChannel = await user.CreateDMChannelAsync();
                    var estateData = _dataFilter.SurfacePriceFilter(
                        dbUser.MinPrice,
                        dbUser.MaxPrice,
                        dbUser.MinSurface,
                        dbUser.MaxSurface
                    );

                    var messages = PrepareMessages(estateData);

                    foreach (var message in messages)
                    {
                        await dmChannel.SendMessageAsync(message);
                    }
                }
            }
        }

        private List<string> PrepareMessages(List<Estate> estateData)
        {
            var messages = new List<string>();
            var currentMessage = new StringBuilder();

            foreach (var estate in estateData)
            {
                string estateInfo = $"{estate.Address} - {estate.Price:C} - {estate.Link}\n";

                if (currentMessage.Length + estateInfo.Length > 2000)
                {
                    messages.Add(currentMessage.ToString());
                    currentMessage.Clear();
                }

                currentMessage.Append(estateInfo);
            }

            if (currentMessage.Length > 0)
            {
                messages.Add(currentMessage.ToString());
            }

            return messages;
        }
    }
}
