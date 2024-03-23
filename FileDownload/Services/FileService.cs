using Microsoft.AspNetCore.Mvc;

namespace FileDownload.Services
{
    public interface IFileService
    {
        Task UploadFile(IFormFileCollection fileCollection);
        Task<(byte[], string)> ReturnFileByteArray(string fileName);
        Task<(FileStream, string)> ReturnFileStream(string fileName);
    }

    public class FileService : IFileService
    {
        readonly IHostEnvironment env;
        readonly IConfiguration Configuration;
        readonly IDictionary<string, string> ContentTypes;

        public FileService(IHostEnvironment env, IConfiguration configuration)
        {
            this.env = env;
            Configuration = configuration;
            ContentTypes = Configuration.GetSection("ContentTypes")
                                        .GetChildren()
                                        .Select(s => new KeyValuePair<string, string>(s["Extension"], s["ContentType"]))
                                        .ToDictionary(s => s.Key, s => s.Value);
        }

        public async Task<(byte[], string)> ReturnFileByteArray(string fileName)
        {
            var filePath = $"{env.ContentRootPath}/wwwroot/{fileName}";

            var fileBytes = await File.ReadAllBytesAsync(filePath);

            var contentType = ContentTypes.FirstOrDefault(f => f.Key == Path.GetExtension(fileName)).Value;

            return (fileBytes, contentType);
        }

        public Task<(FileStream, string)> ReturnFileStream(string fileName)
        {
            var filePath = $"{env.ContentRootPath}/wwwroot/{fileName}";

            var fileStream = File.OpenRead(filePath);

            var contentType = ContentTypes.FirstOrDefault(f => f.Key == Path.GetExtension(fileName)).Value;

            return Task.FromResult((fileStream, contentType));
        }

        public async Task UploadFile(IFormFileCollection fileCollection)
        {
            await Parallel.ForEachAsync(fileCollection, (file, ct) =>
            {
                var filePath = $"{env.ContentRootPath}/wwwroot/{Guid.NewGuid}{Path.GetExtension(file.FileName)}";
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyToAsync(fileStream);
                }

                return ValueTask.CompletedTask;
            });
        }
    }
}
