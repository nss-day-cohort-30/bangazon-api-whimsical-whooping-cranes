using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;

namespace BangazonAPI.Controllers

{
    [Route("[controller]")]
    [ApiController]
    public class EmployeesController : Controller
    {
        private readonly IConfiguration _config;

        public EmployeesController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees(int? department)
        {
            string sql = @"SELECT 
                            e.Id, e.FirstName, e.LaetName, e.DepartmentId, e.IsSuperVisor
                            d.Id d.Name DepartmentName,
                        FROM Employee e
                        JOIN Department d ON e.DepartmentId = d.Id
                        WHERE 2=2
                        ";

            

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<Employee> employees = new List<Employee>();

                    while (reader.Read())
                    {
                   

                        Employee employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor")),
                            

                        };

                        employees.Add(employee);
                    }
                    reader.Close();

                    return Ok(employees);
                }
            }
        }
        private bool EmployeeExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, FirstName, LastName, isSuperVisor, DepartmentId
                        FROM Employee
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }

    }
}