using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using AdminApp.Base.Entities;
using AdminApp.Core.Service;
using AdminApp.Token.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AdminApp.Token.Controllers.v1
{
    [Route("api/v1")]
    [ApiController]
    public class V1Controller : ControllerBase
    {
        /// <summary>
        /// Json Web Tokne 配置项
        /// </summary>
        private JwtSettings JwtSettings { get; set; }
        public V1Controller(IOptions<JwtSettings> settings)
        {
            JwtSettings=settings.Value;
        }
        /// <summary>
        /// Post获取应用授权
        /// </summary>
        /// <param name="user">用户登录信息</param>
        /// <param name="audience">来自的应用</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Post(LoginEntity user,string audience)
        {
            if (string.IsNullOrEmpty(user.UId)||string.IsNullOrEmpty(user.SecurityKey))
            {
                return StatusCode(401,new { Error = "用户名或密码不能为空" });
            }
            if (string.IsNullOrEmpty(audience))
                return StatusCode(401,new { Error = "应用不能为空" });
            var entity = new ServiceContext<Account>().GetEntity(new { LoginName = user.UId,Password = user.SecurityKey,InUse = 1 },"Login");
            if (entity==null)
                return StatusCode(401,new { Error = "用户名或密码错误" });
            return StatusCode(200,CreateToken(entity,audience));
        }
        /// <summary>
        /// Post方式直接指定URL获取应用授权
        /// </summary>
        /// <param name="user">用户登录信息</param>
        /// <param name="audience">来自的应用</param>
        /// <returns></returns>
        [HttpPost("{audience}")]
        public IActionResult PostWithAudience(LoginEntity user,string audience)
        {
            if (string.IsNullOrEmpty(user.UId)||string.IsNullOrEmpty(user.SecurityKey))
            {
                return StatusCode(401,new { Error = "用户名或密码不能为空" });
            }
            if (string.IsNullOrEmpty(audience))
                return StatusCode(401,new { Error = "应用不能为空" });
            var entity = new ServiceContext<Account>().GetEntity(new { LoginName = user.UId,Password = user.SecurityKey,InUse = 1 },"Login");
            if (entity==null)
                return StatusCode(401,new { Error = "用户名或密码错误" });
            return StatusCode(200,CreateToken(entity,audience));
        }
        /// <summary>
        /// Get方式获取应用授权
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="securitykey"></param>
        /// <param name="audience">来自的应用</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get(string uid,string securitykey,string audience)
        {
            if (string.IsNullOrEmpty(uid)||string.IsNullOrEmpty(securitykey))
            {
                return StatusCode(401,new { Error = "用户名或密码不能为空" });
            }
            if (string.IsNullOrEmpty(audience))
                return StatusCode(401,new { Error = "应用不能为空" });
            var entity = new ServiceContext<Account>().GetEntity(new { LoginName = uid,Password = securitykey,InUse = 1 },"Login");
            if (entity==null)
                return StatusCode(401,new { Error = "用户名或密码错误" });
            return StatusCode(200,CreateToken(entity,audience));
        }
        /// <summary>
        /// Get方式直接指定URL请求应用授权
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="securitykey"></param>
        /// <param name="audience"></param>
        /// <returns></returns>
        [HttpGet("{audience}")]
        public IActionResult GetWithAudience(string uid,string securitykey,string audience)
        {
            if (string.IsNullOrEmpty(uid)||string.IsNullOrEmpty(securitykey))
            {
                return StatusCode(401,new { Error = "用户名或密码不能为空" });
            }
            if (string.IsNullOrEmpty(audience))
                return StatusCode(401,new { Error = "应用不能为空" });
            var entity = new ServiceContext<Account>().GetEntity(new { LoginName = uid,Password = securitykey,InUse = 1 },"Login");
            if (entity==null)
                return StatusCode(401,new { Error = "用户名或密码错误" });
            return StatusCode(200,CreateToken(entity,audience));
        }

        /// <summary>
        /// 根据上一个应用换取新应用授权
        /// </summary>
        /// <param name="audience"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("refresh/{audience}")]
        public IActionResult Refresh(string audience)
        {
            var id = User.Claims.ToList().FirstOrDefault(x => x.Type=="uid")?.Value;
            if (string.IsNullOrEmpty(id))
            {
                return StatusCode(401,new { Error = "切换授权失败" });
            }
            var entity = new ServiceContext<Account>().GetEntity(id);
            return StatusCode(200,CreateToken(entity,audience));
        }

        private dynamic CreateToken(Account user,string audience)
        {
            ClaimsIdentity cliaims = new ClaimsIdentity(new Claim [ ]
            {
                new Claim("uid", user.Id),
                new Claim("uname", user.DisplayName)
            });
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(JwtSettings.Key);
            var authTime = DateTime.UtcNow;
            var expiresAt = authTime.AddDays(JwtSettings.Expires);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject=cliaims,
                Issuer=JwtSettings.Issuer,
                Audience=audience,
                Expires=expiresAt,
                SigningCredentials=new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
            };
            return new
            {
                access_token = tokenHandler.CreateEncodedJwt(tokenDescriptor),
                token_type = "Bearer",
                profile = new
                {
                    auth_time = new DateTimeOffset(authTime).ToUnixTimeSeconds(),
                    expires_at = new DateTimeOffset(expiresAt).ToUnixTimeSeconds()
                }
            };
        }
    }
}
