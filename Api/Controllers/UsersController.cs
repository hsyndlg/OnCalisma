using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Api.Model;
using System.Data;
using Dapper;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UsersController : ControllerBase
    {
        private readonly string Constr = "Server=HUSEYIN\\SQLEXPRESS;Database=OnCalisma;User Id=sa;Password=123456";
        [HttpGet]
        public IActionResult GetById(int id)
        {
            string sql = $"select * from Users where Id={id}";
            using (var connection = new SqlConnection(Constr))
            {
                var user = connection.Query<User>(sql,commandType: CommandType.Text).FirstOrDefault();
                if (user!=null)
                {
                    return Ok(user);
                }
                else
                {
                    return BadRequest("başarısız işlem");
                }
            }
            
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            string sql = "select * from Users";
            using (var connection = new SqlConnection(Constr))
            {
                var users = connection.Query<User>(sql, commandType: CommandType.Text).ToList();
                if (users!=null)
                {
                    return Ok(users);
                }
                else
                {
                    return BadRequest("işlem başarısız");
                }
            }
            
        }
        [HttpPost]
        public IActionResult Login(string userName,string password)
        {
            string sql = $"select * from Users where UserName='@username' and Password='@password'";
            using (var connection = new SqlConnection(Constr))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@username",userName,DbType.String);
                parameters.Add("@password",password,DbType.String);
                var result = connection.Query<User>(sql);
                if (result != null) return Ok(result);
                else return BadRequest("giriş başarısız");
            }
        }
    }
}