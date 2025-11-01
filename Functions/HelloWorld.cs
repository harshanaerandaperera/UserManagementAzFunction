using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UserManagementAzFunction;

/// <summary>
/// HelloWorld Azure Function demonstrating AspNetCore HTTP bindings integration.
/// This function uses ASP.NET Core's HttpRequest and IActionResult for a more familiar
/// development experience for ASP.NET Core developers.
/// </summary>
public class HelloWorld
{
    private readonly ILogger<HelloWorld> _logger;

    public HelloWorld(ILogger<HelloWorld> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// HTTP GET endpoint that returns a welcome message.
    /// Uses AspNetCore HTTP bindings (HttpRequest/IActionResult) instead of Worker HTTP bindings (HttpRequestData/HttpResponseData).
    /// </summary>
    /// <param name="req">HttpRequest object from ASP.NET Core, provides familiar request handling</param>
    /// <returns>IActionResult allowing use of built-in result types like OkObjectResult, BadRequestResult, etc.</returns>
    [Function("HelloWorld")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req) 
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        
        // Query parameters are accessed directly from HttpRequest.Query (ASP.NET Core style)
        string? name = req.Query["name"];
        
        // Build response message based on query parameter
        string responseMessage = string.IsNullOrEmpty(name) 
            ? "Welcome to Azure Functions! CI/CD is working! 🚀" 
            : $"Hello, {name}! CI/CD Pipeline is successfully deployed! 🎉";
        
        // Return IActionResult - automatic JSON serialization and HTTP 200 status
        // Other options: BadRequestResult, NotFoundResult, CreatedResult, etc.
        return new OkObjectResult(responseMessage);
    }
}
