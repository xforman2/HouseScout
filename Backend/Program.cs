using Backend.Clients;
using Backend.Mappers;
using Backend.Seeding;
using Backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedDependencies.Model;
using SharedDependencies.Services;

class Program
{
    static async Task Main(string[] args)
    {
        // Build the host
        using var host = CreateHostBuilder().Build();

        // Main scope of application
        using (var scope = host.Services.CreateScope())
        {
            
            var services = scope.ServiceProvider;
            
            var dataProcessingService = services.GetRequiredService<DataProcessingService>();
            await dataProcessingService.ProcessData();
        }
    }

    private static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .ConfigureServices(
                (context, services) =>
                {
                    var configuration = context.Configuration;

                    services
                        .AddSingleton(configuration)
                        .AddDbContext<HouseScoutContext>(options =>
                            options.UseNpgsql(
                                configuration.GetConnectionString("DefaultConnection")
                            )
                        )
                        .AddSingleton<BezrealitkyGraphQLClient>()
                        .AddSingleton<SrealityHttpClient>()
                        .AddSingleton<BezrealitkyMapper>()
                        .AddSingleton<SrealityMapper>()
                        .AddScoped<DbSeeder>()
                        .AddSingleton<Dictionary<IClient, IMapper>>(provider =>
                        {
                            var srealityClient = provider.GetRequiredService<SrealityHttpClient>();
                            var bezrealitkyClient =
                                provider.GetRequiredService<BezrealitkyGraphQLClient>();
                            var srealityMapper = provider.GetRequiredService<SrealityMapper>();
                            var bezrealitkyMapper =
                                provider.GetRequiredService<BezrealitkyMapper>();

                            return new Dictionary<IClient, IMapper>
                            {
                                { srealityClient, srealityMapper },
                                { bezrealitkyClient, bezrealitkyMapper },
                            };
                        })
                        .AddScoped<DataProcessingService>()
                        .AddSingleton<RabbitMQService>();
                }
            );
}
