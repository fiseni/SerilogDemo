using Microsoft.EntityFrameworkCore;
using Serilog;
using SerilogDemo.Web.Data;

// If you need to log or catch exceptions during the build phase and app startup,
// then enclose the whole content within the try/catch block and use Two-stage initialization.
// Refer to https://github.com/serilog/serilog-aspnetcore

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureSerilog();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<DbInitializer>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();


public static class LoggingExtensions
{
    // For ASP.NET Core apps, moving forward this is the canonical way to configure Serilog. Do not use builder.Host.UseSerilog().
    // This will set up Serilog as the only logging provider, it replaces the default logging provider.
    public static WebApplicationBuilder ConfigureSerilog(this WebApplicationBuilder builder)
    {
        builder.Services.AddSerilog((services, lc) => lc
            .ReadFrom.Configuration(builder.Configuration)
            .ReadFrom.Services(services));

        return builder;
    }

    // If you do it this way, the UseSerilogRequestLogging doesn't work correctly.
    public static WebApplicationBuilder ConfigureSerilog2(this WebApplicationBuilder builder)
    {
        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        builder.Services.AddSerilog(logger);

        return builder;
    }

    // This adds the Serilog just as a provider to the existing logging infrastructure.
    public static WebApplicationBuilder ConfigureSerilog3(this WebApplicationBuilder builder)
    {
        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(logger);

        return builder;
    }

}
