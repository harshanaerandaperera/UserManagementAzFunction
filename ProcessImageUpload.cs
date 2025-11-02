using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace MyFirstAzureFunction
{
    public class ProcessImageUpload
    {
        private readonly ILogger _logger;

        public ProcessImageUpload(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ProcessImageUpload>();
        }

        [Function("ProcessImageUpload")]
        public async Task Run(
            [BlobTrigger("uploads/{name}", Connection = "AzureWebJobsStorage")] Stream blobStream,
            string name)
        {
            _logger.LogInformation($"Blob trigger processing file: {name}, Size: {blobStream.Length} bytes");

            try
            {
                // Only process image files
                var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                var extension = Path.GetExtension(name).ToLower();

                if (!imageExtensions.Contains(extension))
                {
                    _logger.LogInformation($"Skipping non-image file: {name}");
                    return;
                }

                // Load the image
                using var image = await Image.LoadAsync(blobStream);
                
                _logger.LogInformation($"Original image size: {image.Width}x{image.Height}");

                // Create thumbnail (200x200)
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(200, 200),
                    Mode = ResizeMode.Max
                }));

                _logger.LogInformation($"Thumbnail size: {image.Width}x{image.Height}");

                // Save thumbnail to blob storage
                var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
                var blobServiceClient = new BlobServiceClient(connectionString);
                var thumbnailContainer = blobServiceClient.GetBlobContainerClient("thumbnails");

                var thumbnailName = $"thumb_{name}";
                var thumbnailBlob = thumbnailContainer.GetBlobClient(thumbnailName);

                using (var thumbnailStream = new MemoryStream())
                {
                    await image.SaveAsync(thumbnailStream, new JpegEncoder());
                    thumbnailStream.Position = 0;
                    await thumbnailBlob.UploadAsync(thumbnailStream, overwrite: true);
                }

                _logger.LogInformation($"Thumbnail created and saved: {thumbnailName}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing image {name}: {ex.Message}");
            }
        }
    }
}