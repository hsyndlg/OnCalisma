using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Api.Model;
using System.Data;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Controllers
{
    // Requestten gelmeyen cookie ve swagger ile gelen cookieler
    // AJAX'ta dönen response'ı html'de bastırma.
    // Identity mekanizmasının dapper'la kullanımı.
    [ApiController]
    
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public UsersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private readonly string Constr = "Server=HUSEYIN\\SQLEXPRESS;Database=OnCalisma;User Id=sa;Password=123456";
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            string sql = $"select * from Users where Id={id}";
            if(HttpContext.Request.Cookies["TokenUserCookie"] != null)
            {
                using (var connection = new SqlConnection(Constr))
                {
                    var user = connection.Query<User>(sql,commandType: CommandType.Text).FirstOrDefault();
                    if (user!=null)
                    {
                        return Ok(user);
                    }
                    else
                    {
                        return BadRequest("Kullanıcı bulunamadı.");
                    }
                }
            }
            else
            {
                return RedirectToAction("AccessDenied");
            }
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            string sql = "select * from Users";
            if(HttpContext.Request.Cookies["TokenUserCookie"] != null)
            {
                using (var connection = new SqlConnection(Constr))
                {
                    var users = connection.Query<User>(sql, commandType: CommandType.Text).ToList();
                    if (users!=null)
                    {
                        return Ok(users);
                    }
                    else
                    {
                        return BadRequest("Kullanıcı bulunamadı.");
                    }
                }
            }
            else
            {
                return RedirectToAction("AccessDenied");
            }
        }
        [HttpPost]
        public IActionResult Create(User user)
        {
            string sql = $"insert into users (UserName,Password,Name,SurName) values (@Username,@Password,@Name,@Surname)";
            if(HttpContext.Request.Cookies["TokenUserCookie"] != null)
            {
                using (var connection = new SqlConnection(Constr))
                {
                    var result = connection.Execute(sql,new {
                        Username = user.Name,
                        Password = user.Password,
                        Name = user.Name,
                        Surname = user.Surname
                    });
                    if (result > 0) return Ok();
                    else return BadRequest("Kullanıcı bulunamadı.");
                }
            }
            else
            {
                return RedirectToAction("AccessDenied");
            }
        }
        [HttpPut]
        public IActionResult Update(User user)
        {
            string sql = $"update users set UserName=@Username,Password=@Password,Name=@Name,SurName=@Surname where Id=@id";
            if(HttpContext.Request.Cookies["TokenUserCookie"] != null)
            {
                using (var connection = new SqlConnection(Constr))
                {
                    var result = connection.Execute(sql,new {
                        id = user.Id,
                        Username = user.Name,
                        Password = user.Password,
                        Name = user.Name,
                        Surname = user.Surname
                    });
                    if (result > 0) return Ok();
                    else return BadRequest("Kullanıcı bulunamadı.");
                }
            }
            else
            {
                return RedirectToAction("AccessDenied");
            }
        }
        [HttpDelete]
        public IActionResult Delete(User user)
        {
            string sql = "delete from users where Id=@ID";
            if(HttpContext.Request.Cookies["TokenUserCookie"] != null)
            {
                using (var connection = new SqlConnection(Constr))
                {
                    var result = connection.Execute(sql,new { 
                        ID = user.Id
                    });
                    if (result > 0) return Ok();
                    else return BadRequest("İşlem başarısız");
                }
            }
            else
            {
                return RedirectToAction("AccessDenied");
            }
        }
        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> Login(User user)
        {
            string sql = $"select * from users where Username='{user.Username}' and Password='{user.Password}'";
            using (IDbConnection connection = new SqlConnection(Constr))
            {
                var result = connection.QueryFirstOrDefault<User>(sql);

                if (result != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,user.Username),
                        new Claim(ClaimTypes.Surname,user.Password)
                    };
                    var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authenticationProperty = new AuthenticationProperties
                    {
                        RedirectUri = @"/index.html"
                    };


                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme
                                            , new ClaimsPrincipal(claimIdentity)
                                            , authenticationProperty);
                    return Ok(result);
                }
                else
                {
                    return NotFound("Kullanıcı bulunamadı");
                }
            }
        }
        [HttpPost("[action]")]
        public IActionResult Logout(User user)
        {
            if (HttpContext.Request.Cookies["TokenUserCookie"] != null)
            {
                return Ok("Çıkış yapıldı.");
            }
            else
            {
                return BadRequest("Giriş yapılmamış");
            }
        }
        [HttpGet("[action]")]
        public IActionResult AccessDenied()
        {
            return Redirect("~/accessdenied.html");
        }
    }
}