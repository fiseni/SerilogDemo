using Microsoft.EntityFrameworkCore;

namespace SerilogDemo.Web.Data;

public class DbInitializer : IHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public DbInitializer(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await context.Database.EnsureCreatedAsync(cancellationToken);

        if (await context.Customers.AnyAsync(cancellationToken)) return;

        var customers = new List<Customer>
        {
            new() { Name = "Customer1", Email = "email1@local.com" },
            new() { Name = "Customer2", Email = "email2@local.com" },
            new() { Name = "Customer3", Email = "email3@local.com" },
        };

        context.Customers.AddRange(customers);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
