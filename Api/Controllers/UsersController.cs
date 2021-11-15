using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetById(int id)
        {
            return Ok();
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            string sql = "select * from Users";
            using (var connection = new SqlConnection())
            {
                 
            }
            return Ok();
        }
    }
}