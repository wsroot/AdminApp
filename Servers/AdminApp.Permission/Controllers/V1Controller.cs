using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminApp.Permission.Controllers
{
    [Route("api/v1")]
    [Authorize]
    [ApiController]
    public class V1Controller : ControllerBase
    {
        //应用 菜单 按钮 数据
        // GET api/values

        [HttpGet]
        [Route("application")]
        public ActionResult<IEnumerable<string>> GetApplicationList()
        {
            return new string [ ] { "value1","value2" };
        }

        [HttpGet]
        [Route("menu")]
        public ActionResult<IEnumerable<string>> GetMenuList()
        {
            return new string [ ] { "value1","value2" };
        }

        [HttpGet]
        [Route("operation")]
        public ActionResult<IEnumerable<string>> GetOperationList()
        {
            return new string [ ] { "value1","value2" };
        }
    }
}
