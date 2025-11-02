using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;

namespace MyFirstAzureFunction
{
    public class ListFiles
    {
        private readonly ILogger _logger;

        public ListFiles(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ListFiles>();
        }

        [Function("ListFiles")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            _logger.LogInformation("Listing all files");

            try
            {
                var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
                var containerName = "uploads";

                var blobServiceClient = new BlobServiceClient(connectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                var files = new List<FileInfo>();

                await foreach (var blobItem in containerClient.GetBlobsAsync())
                {
                    var blobClient = containerClient.GetBlobClient(blobItem.Name);
                    var properties = await blobClient.GetPropertiesAsync();

                    files.Add(new FileInfo
                    {
                        Name = blobItem.Name,
                        Size = blobItem.Properties.ContentLength ?? 0,
                        CreatedOn = blobItem.Properties.CreatedOn?.DateTime ?? DateTime.MinValue,
                        Url = blobClient.Uri.ToString(),
                        ContentType = properties.Value.ContentType
                    });
                }

                _logger.LogInformation($"Found {files.Count} files");

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(new { count = files.Count, files = files });
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

    public class FileInfo
    {
        public string Name { get; set; }
        public long Size { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Url { get; set; }
        public string ContentType { get; set; }
    }
}