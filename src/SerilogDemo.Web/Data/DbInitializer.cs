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

        var dbCreated = await context.Database.EnsureCreatedAsync(cancellationToken);

        // If it already exists then exit, do not seed anything.
        if (!dbCreated) return;

        var logTableSql = """
            CREATE TABLE [Logs] (
               [Id] int IDENTITY(1,1) NOT NULL,
               [Message] nvarchar(max) NULL,
               [MessageTemplate] nvarchar(max) NULL,
               [Level] nvarchar(128) NULL,
               [TimeStamp] datetime NOT NULL,
               [Exception] nvarchar(max) NULL,
               [LogEvent] nvarchar(max) NULL,
               [ProcessName] varchar(100) NULL,
               [UserId] varchar(100) NULL,
               [TraceId] varchar(100) NULL,
               [SpanId] varchar(100) NULL,

               CONSTRAINT [PK_Logs] PRIMARY KEY CLUSTERED ([Id] ASC)
            );
            """;

        await context.Database.ExecuteSqlRawAsync(logTableSql, cancellationToken);

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
