using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> TestDb([FromServices] AppDbContext dbContext, CancellationToken cancellationToken)
    {
        // Testing EF log outputs.

        var customers = await dbContext.Customers.ToListAsync(cancellationToken);

        return Ok(customers);
    }
}
