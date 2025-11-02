using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using System.Text.Json;

namespace MyFirstAzureFunction
{
    public class UploadFile
    {
        private readonly ILogger _logger;

        public UploadFile(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<UploadFile>();
        }

        [Function("UploadFile")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("File upload request received");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var uploadRequest = JsonSerializer.Deserialize<FileUploadRequest>(requestBody);

                if (uploadRequest == null || string.IsNullOrEmpty(uploadRequest.FileName) || string.IsNullOrEmpty(uploadRequest.FileContent))
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteAsJsonAsync(new { error = "FileName and FileContent are required" });
                    return badResponse;
                }

                // Get connection string
                var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
                var containerName = "uploads";

                // Create blob client
                var blobServiceClient = new BlobServiceClient(connectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                
                // Generate unique blob name
                var blobName = $"{Guid.NewGuid()}_{uploadRequest.FileName}";
                var blobClient = containerClient.GetBlobClient(blobName);

                // Convert base64 to bytes and upload
                byte[] fileBytes = Convert.FromBase64String(uploadRequest.FileContent);
                using (var stream = new MemoryStream(fileBytes))
                {
                    await blobClient.UploadAsync(stream, overwrite: true);
                }

                _logger.LogInformation($"File uploaded: {blobName}");

                var response = req.CreateResponse(HttpStatusCode.Created);
                await response.WriteAsJsonAsync(new
                {
                    message = "File uploaded successfully",
                    blobName = blobName,
                    url = blobClient.Uri.ToString(),
                    size = fileBytes.Length
                });
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

    public class FileUploadRequest
    {
        public string FileName { get; set; }
        public string FileContent { get; set; } // Base64 encoded
    }
}