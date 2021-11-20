using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Api.Model;
using System.Data;
using Dapper;
using Newtonsoft.Json;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly string Constr = "Server=HUSEYIN\\SQLEXPRESS;Database=OnCalisma;User Id=sa;Password=123456";
        [HttpGet("{id}")]
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
        public IActionResult Create(User user)
        {
            string sql = $"insert into users (UserName,Password,Name,SurName) values (@Username,@Password,@Name,@Surname)";
            using (var connection = new SqlConnection(Constr))
            {
                var result = connection.Execute(sql,new {
                    Username = user.Name,
                    Password = user.Password,
                    Name = user.Name,
                    Surname = user.SurName
                });
                if (result > 0) return Ok();
                else return BadRequest("İşlem başarısız");
            }
        }
        [HttpPut]
        public IActionResult Update(User user)
        {
            string sql = $"update users set UserName=@Username,Password=@Password,Name=@Name,SurName=@Surname where Id=@id";
            using (var connection = new SqlConnection(Constr))
            {
                var result = connection.Execute(sql,new {
                    id = user.Id,
                    Username = user.Name,
                    Password = user.Password,
                    Name = user.Name,
                    Surname = user.SurName
                });
                if (result > 0) return Ok();
                else return BadRequest("İşlem başarısız");
            }
        }
        [HttpDelete]
        public IActionResult Delete(User user)
        {
            string sql = "delete from users where Id=@ID";
            using (var connection = new SqlConnection(Constr))
            {
                var result = connection.Execute(sql,new { 
                    ID = user.Id
                });
                if (result > 0) return Ok();
                else return BadRequest("İşlem başarısız");
            }
        }
    }
}