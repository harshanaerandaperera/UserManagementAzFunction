using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;

namespace MyFirstAzureFunction
{
    public class DeleteFile
    {
        private readonly ILogger _logger;

        public DeleteFile(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<DeleteFile>();
        }

        [Function("DeleteFile")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "files/{blobName}")] HttpRequestData req,
            string blobName)
        {
            _logger.LogInformation($"Delete request for: {blobName}");

            try
            {
                var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
                var containerName = "uploads";

                var blobServiceClient = new BlobServiceClient(connectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(blobName);

                if (!await blobClient.ExistsAsync())
                {
                    var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                    await notFoundResponse.WriteAsJsonAsync(new { error = "File not found" });
                    return notFoundResponse;
                }

                await blobClient.DeleteAsync();

                _logger.LogInformation($"File deleted: {blobName}");

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(new { message = "File deleted successfully", blobName = blobName });
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteAsJsonAsync(new { error = ex.Message });
                return errorResponse;
            }
        }
    }
}