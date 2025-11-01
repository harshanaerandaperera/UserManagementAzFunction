using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using UserManagementAzFunction.Repositories;

namespace UserManagementAzFunction
{
    public class DeleteUser
    {
        private readonly ILogger _logger;
        private readonly IUserRepository _userRepository;

        public DeleteUser(ILoggerFactory loggerFactory, IUserRepository userRepository)
        {
            _logger = loggerFactory.CreateLogger<DeleteUser>();
            _userRepository = userRepository;
        }

        [Function("DeleteUser")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "users/{id}")] HttpRequestData req,
            string id)
        {
            _logger.LogInformation($"Deleting user with ID: {id}");

            try
            {
                var user = await _userRepository.GetByIdAsync(id);

                if (user == null)
                {
                    var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                    await notFoundResponse.WriteAsJsonAsync(new { error = "User not found" });
                    return notFoundResponse;
                }

                await _userRepository.DeleteAsync(user);

                _logger.LogInformation($"User deleted successfully: {id}");

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(new { message = "User deleted successfully", userId = id });
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
}
