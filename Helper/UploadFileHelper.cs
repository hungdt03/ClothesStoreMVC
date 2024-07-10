using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.Extensions.Options;

namespace WebBanQuanAo.Helper
{
    public class UploadFileHelper
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<UploadFileHelper> _logger;

        public UploadFileHelper(IOptions<CloudinarySettings> config, ILogger<UploadFileHelper> logger)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
            _logger = logger;

        }

        public async Task<List<string>> UploadImagesAsync(List<IFormFile> files)
        {
            var uploadResults = new List<string>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    using (var stream = file.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(file.FileName, stream),
                            Folder = "store-web-app",
                        };

                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                        if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            uploadResults.Add(uploadResult.SecureUrl.ToString());
                        }
                    }
                }
            }

            return uploadResults;
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is invalid.");
            }

            if (!file.ContentType.StartsWith("image/"))
            {
                throw new ArgumentException("File is not an image.");
            }

            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "store-web-app",
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return uploadResult.SecureUrl.ToString();
                }
                else
                {
                    throw new Exception("Image upload failed.");
                }
            }
        }
    }

    public class CloudinarySettings
    {
        public string CloudName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }
}
