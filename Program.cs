using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using HouseScout.Clients;
using HouseScout.Mappers;
using HouseScout.Model;
using Microsoft.EntityFrameworkCore;

class Program
{
    private static DiscordSocketClient _client;
    private static InteractionService _interactionService;

    static async Task Main(string[] args)
    {
        // Build the host
        using var host = CreateHostBuilder().Build();

        // Configure Discord client and interaction service
        _client = host.Services.GetRequiredService<DiscordSocketClient>();
        _interactionService = host.Services.GetRequiredService<InteractionService>();

        _client.Log += LogAsync;
        _interactionService.Log += LogAsync;

        // Register the interaction handler                     
        _client.InteractionCreated += async interaction =>
        {
            var context = new SocketInteractionContext(_client, interaction);
            await _interactionService.ExecuteCommandAsync(context, host.Services);
        };

        // Initialize the client
        var configuration = host.Services.GetRequiredService<IConfiguration>();
        string? token = configuration["DiscordToken"];
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        // Register commands
        _client.Ready += async () =>
        {
            await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), host.Services);
            await _interactionService.RegisterCommandsGloballyAsync();
        };
        
        // Main scope of application
        using (var scope = host.Services.CreateScope())
        {
            //TODO this is just temp, delete later
            var services = scope.ServiceProvider;
            var dbContext = services.GetRequiredService<HouseScoutContext>();

            var bezrealitkyClient = services.GetRequiredService<BezrealitkyGraphQLClient>();
            var srealityClient = services.GetRequiredService<SrealityHttpClient>();

            var bezrealitkyMapper = services.GetRequiredService<BezrealitkyMapper>();
            var srealityMapper = services.GetRequiredService<SrealityMapper>();

            var bezrealitkyResponse = await bezrealitkyClient.GetAdvertsAsync();
            var srealityResponse = await srealityClient.GetSrealityData();

            var bezrealitkyMappedData = bezrealitkyMapper.MapResponseToModel(bezrealitkyResponse);
            var srealityMappedData = srealityMapper.MapResponseToModel(srealityResponse);

            await dbContext.Estates.AddRangeAsync(bezrealitkyMappedData);
            await dbContext.Estates.AddRangeAsync(srealityMappedData);

            await dbContext.SaveChangesAsync();
        }

        await Task.Delay(-1); // Keep the bot running
    }

    private static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                var configuration = context.Configuration;

                services.AddSingleton(configuration)
                        .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                        {
                            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
                        }))
                        .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                        .AddDbContext<HouseScoutContext>(options =>
                            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")))
                        .AddSingleton<BezrealitkyGraphQLClient>()
                        .AddSingleton<SrealityHttpClient>()
                        .AddSingleton<BezrealitkyMapper>()
                        .AddSingleton<SrealityMapper>();
            });

    private static Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log.ToString());
        return Task.CompletedTask;
    }
}
