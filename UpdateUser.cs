using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace UserManagementAzFunction
{
    public class UpdateUser
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _dbContext;

        public UpdateUser(ILoggerFactory loggerFactory, ApplicationDbContext dbContext)
        {
            _logger = loggerFactory.CreateLogger<UpdateUser>();
            _dbContext = dbContext;
        }

        [Function("UpdateUser")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "users/{id}")] HttpRequestData req,
            string id)
        {
            _logger.LogInformation($"Updating user with ID: {id}");

            try
            {
                var user = await _dbContext.Users.FindAsync(id);

                if (user == null)
                {
                    var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                    await notFoundResponse.WriteAsJsonAsync(new { error = "User not found" });
                    return notFoundResponse;
                }

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var updateRequest = JsonSerializer.Deserialize<UpdateUserRequest>(requestBody);

                if (updateRequest == null)
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteAsJsonAsync(new { error = "Invalid request body" });
                    return badResponse;
                }

                // Update user properties
                if (!string.IsNullOrEmpty(updateRequest.Name))
                    user.Name = updateRequest.Name;
                
                if (!string.IsNullOrEmpty(updateRequest.Email))
                    user.Email = updateRequest.Email;
                
                if (!string.IsNullOrEmpty(updateRequest.PhoneNumber))
                    user.PhoneNumber = updateRequest.PhoneNumber;
                
                if (!string.IsNullOrEmpty(updateRequest.Status))
                    user.Status = updateRequest.Status;

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"User updated successfully: {id}");

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(user);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteAsJsonAsync(new { error = "Internal server error" });
                return errorResponse;
            }
        }
    }

    public class UpdateUserRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }
    }
}
