﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SerilogDemo.Web.Data;

namespace SerilogDemo.Web;

[ApiController]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;

    public TestController(ILogger<TestController> logger)
    {
        _logger = logger;
    }

    [HttpGet("test")]
    public IActionResult Test()
    {
        _logger.LogInformation("Test method called");

        return Ok();
    }

    [HttpGet("test-error")]
    public IActionResult TestError()
    {
        try
        {
            var x = 0;
            _ = 1 / x;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Test error called");
        }

        return Ok();
    }

    [HttpGet("test-db")]
    public async Task<IActionResult> TestDb(int? age, [FromServices] AppDbContext dbContext, CancellationToken cancellationToken)
    {
        // Testing EF log outputs.

        age ??= 30;

        var customers = await dbContext.Customers
            .Where(x => x.Age > age)
            .ToListAsync(cancellationToken);

        // Testing the output format of serialized complex objects.
        _logger.LogInformation("Test db called, customers: {@customers}", customers);

        return Ok(customers);
    }
}
