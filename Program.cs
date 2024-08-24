using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using System.Reflection;

using Microsoft.EntityFrameworkCore;

class Program
{
    private static DiscordSocketClient _client;
    private static InteractionService _interactionService;
    private static IServiceProvider _services;
    private static IConfiguration _configuration;

    static async Task Main(string[] args)
    {
        // Load the configuration
        _configuration = BuildConfig();

        _services = ConfigureServices();

        _client = _services.GetRequiredService<DiscordSocketClient>();
        _interactionService = _services.GetRequiredService<InteractionService>();

        _client.Log += LogAsync;
        _interactionService.Log += LogAsync;

        // Register the interaction handler                     
        _client.InteractionCreated += async interaction =>
        {
            var context = new SocketInteractionContext(_client, interaction);
            await _interactionService.ExecuteCommandAsync(context, _services);
        };

        // Initialize the client
        string? token = _configuration["DiscordToken"];
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        // Register commands
        _client.Ready += async () =>
        {
            await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            await _interactionService.RegisterCommandsGloballyAsync();
        };

        await Task.Delay(-1); // Keep the bot running
    }

    private static IServiceProvider ConfigureServices()
    {
        return new ServiceCollection()
            .AddSingleton(_configuration)
            .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            }))
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddDbContext<DbContext>(options =>
                options.UseNpgsql(_configuration.GetConnectionString("DefaultConnection")))
            .BuildServiceProvider();
    }

    private static IConfiguration BuildConfig()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        return builder.Build();
    }

    private static Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log.ToString());
        return Task.CompletedTask;
    }
}
