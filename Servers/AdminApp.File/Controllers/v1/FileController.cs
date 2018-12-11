using AdminApp.File.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdminApp.File.Controllers.v1
{
    [Authorize]
    [ApiController]
    [Route("api/v1/file")]
    [Produces("application/json")]
    [Consumes("application/json","multipart/form-data")]
    public class FileController : ControllerBase
    {
        [HttpPost("static")]
        public IActionResult PostStaticFile(IFormFileCollection files)
        {
            var requestFiles = Request.Form.Files;
            return Ok();
        }

        [HttpPost]
        public IActionResult PostFile(FileData fileData,IFormFileCollection files)
        {
            var requestFiles = Request.Form.Files;
            return Ok();
        }
    }
}
