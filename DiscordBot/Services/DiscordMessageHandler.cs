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
        private readonly IDbContextFactory<HouseScoutContext> _contextFactory;
        private readonly DataFilter _dataFilter;

        public DiscordMessageHandler(
            DiscordSocketClient client,
            DataFilter dataFilter,
            IDbContextFactory<HouseScoutContext> contextFactory
        )
        {
            _client = client;
            _dataFilter = dataFilter;
            _contextFactory = contextFactory;
        }

        public async Task HandleMessageAsync()
        {
            await using (var context = await _contextFactory.CreateDbContextAsync())
            {
                var users = await context.Users.ToListAsync();
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
                            dbUser.MaxSurface,
                            dbUser.IsNew
                        );

                        if (dbUser.IsNew)
                        {
                            dbUser.IsNew = false;
                        }

                        var messages = PrepareMessages(estateData);

                        foreach (var message in messages)
                        {
                            await dmChannel.SendMessageAsync(message);
                        }
                    }
                }
                await context.SaveChangesAsync();
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
