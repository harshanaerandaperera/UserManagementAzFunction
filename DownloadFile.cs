using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;

namespace MyFirstAzureFunction
{
    public class DownloadFile
    {
        private readonly ILogger _logger;

        public DownloadFile(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<DownloadFile>();
        }

        [Function("DownloadFile")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "files/{blobName}")] HttpRequestData req,
            string blobName)
        {
            _logger.LogInformation($"Download request for: {blobName}");

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

                var download = await blobClient.DownloadAsync();
                
                using (var memoryStream = new MemoryStream())
                {
                    await download.Value.Content.CopyToAsync(memoryStream);
                    var fileBytes = memoryStream.ToArray();

                    var response = req.CreateResponse(HttpStatusCode.OK);
                    response.Headers.Add("Content-Type", "application/octet-stream");
                    response.Headers.Add("Content-Disposition", $"attachment; filename={blobName}");
                    await response.Body.WriteAsync(fileBytes);
                    
                    _logger.LogInformation($"File downloaded: {blobName}");
                    return response;
                }
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