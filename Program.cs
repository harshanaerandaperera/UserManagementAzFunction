using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserManagementAzFunction.Data;
using UserManagementAzFunction.Repositories;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // Register database context
        var connectionString = Environment.GetEnvironmentVariable("SqlConnectionString") 
            ?? throw new InvalidOperationException("SqlConnectionString environment variable is not set.");
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
    })
    .Build();

host.Run();
