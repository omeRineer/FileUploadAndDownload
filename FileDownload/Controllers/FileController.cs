using FileDownload.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileDownload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("upload-file")]
        public async Task<IActionResult> UploadFiles(IFormFileCollection files)
        {
            await _fileService.UploadFile(files);

            return Ok();
        }

        [HttpGet("file-bytes")]
        public async Task<IActionResult> ReturnFileByteArray(string fileName)
        {
            var (fileBytes, contentType) = await _fileService.ReturnFileByteArray(fileName);

            return File(fileBytes, contentType, fileDownloadName: fileName);
        }

        [HttpGet("file-stream")]
        public async Task<IActionResult> ReturnFileStream(string fileName)
        {
            var (fileStream, contentType) = await _fileService.ReturnFileStream(fileName);

            return File(fileStream, contentType, fileDownloadName: fileName);

        }
    }
}
