using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using UserManagementAzFunction.Models;
using UserManagementAzFunction.Repositories;

namespace UserManagementAzFunction
{
    public class UserRegistration
    {
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;

        public UserRegistration(ILoggerFactory loggerFactory, IUserRepository userRepository)
        {
            _logger = loggerFactory.CreateLogger<UserRegistration>();
            _userRepository = userRepository;
        }

        [Function("RegisterUser")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("User registration request received");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var userRequest = JsonSerializer.Deserialize<UserRegistrationRequest>(requestBody);

                if (userRequest == null || string.IsNullOrEmpty(userRequest.Email))
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteAsJsonAsync(new { error = "Email is required" });
                    return badResponse;
                }

                // Create user and save to database
                var user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = userRequest.Email,
                    Name = userRequest.Name,
                    PhoneNumber = userRequest.PhoneNumber,
                    RegistrationDate = DateTime.UtcNow,
                    Status = "Active"
                };

                await _userRepository.CreateAsync(user);

                _logger.LogInformation($"User saved to database: {user.Id}");

                var response = req.CreateResponse(HttpStatusCode.Created);
                await response.WriteAsJsonAsync(new UserRegistrationResponse
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    RegistrationDate = user.RegistrationDate,
                    Status = user.Status
                });
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

    public class UserRegistrationRequest
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class UserRegistrationResponse
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Status { get; set; }
    }
}
