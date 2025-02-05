﻿using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordBot.Filters;
using DiscordBot.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedDependencies.Model;
using SharedDependencies.Services;

namespace DiscordBot;

class Program
{
    private static DiscordSocketClient _client;
    private static InteractionService _interactionService;
    private static CommandService _commandService;

    static async Task Main(string[] args)
    {
        // Build the host
        using var host = CreateHostBuilder().Build();

        // Configure Discord client and interaction service
        _client = host.Services.GetRequiredService<DiscordSocketClient>();
        _interactionService = host.Services.GetRequiredService<InteractionService>();
        _commandService = host.Services.GetRequiredService<CommandService>();

        _client.Log += LogAsync;
        _interactionService.Log += LogAsync;
        _commandService.Log += LogAsync;

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

        var rabbitMqService = host.Services.GetRequiredService<RabbitMQService>();
        rabbitMqService.StartListening();

        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<HouseScoutContext>();
            if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                await context.Database.MigrateAsync();
            }
        }

        await Task.Delay(-1); // Keep the bot running
    }

    private static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .ConfigureServices(
                (context, services) =>
                {
                    var configuration = context.Configuration;

                    services
                        .AddSingleton(configuration)
                        .AddSingleton(
                            new DiscordSocketClient(
                                new DiscordSocketConfig
                                {
                                    GatewayIntents =
                                        GatewayIntents.AllUnprivileged
                                        | GatewayIntents.MessageContent,
                                }
                            )
                        )
                        .AddSingleton(x => new InteractionService(
                            x.GetRequiredService<DiscordSocketClient>()
                        ))
                        .AddDbContext<HouseScoutContext>(options =>
                            options.UseNpgsql(
                                configuration.GetConnectionString("DefaultConnection")
                            )
                        )
                        .AddDbContextFactory<HouseScoutContext>(options =>
                            options.UseNpgsql(
                                configuration.GetConnectionString("DefaultConnection")
                            )
                        )
                        .AddSingleton<CommandService>()
                        .AddScoped<DataFilter>()
                        .AddSingleton<RabbitMQService>()
                        .AddSingleton<IMessageHandler, DiscordMessageHandler>();
                }
            );

    private static Task LogAsync(LogMessage message)
    {
        if (message.Exception is CommandException cmdException)
        {
            Console.WriteLine(
                $"[Command/{message.Severity}] {cmdException.Command.Aliases.First()}"
                    + $" failed to execute in {cmdException.Context.Channel}."
            );
            Console.WriteLine(cmdException);
        }
        else
            Console.WriteLine($"[General/{message.Severity}] {message}");

        return Task.CompletedTask;
    }
}
