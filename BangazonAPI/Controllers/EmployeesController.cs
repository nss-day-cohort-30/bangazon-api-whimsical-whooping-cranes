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

/*
    Purpose: Controller for Employee  Class
    Author: Abbey Brown
    Methods: Get single, Get all, Post, and Put methods

 */

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

        //allows user to get all the computers from the database

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            string sql = @"SELECT
                            e.Id, e.FirstName, e.LastName, e.DepartmentId, e.IsSuperVisor, 
                            d.Id, d.Name DepartmentName, d.Budget
                            FROM Employee e
                            JOIN Department d ON e.DepartmentId = d.Id
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
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor")),
                            department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Name = reader.GetString(reader.GetOrdinal("DepartmentName")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                            }

                        };

                        employees.Add(employee);
                    }
                    reader.Close();

                    return Ok(employees);
                }
            }
        }

        //allows user to get single computer from the database
        //accepts the specific employee id as an argument

        [HttpGet("{id}", Name = "GetEmployee")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {


            string sql = @"SELECT
                            e.Id, e.FirstName, e.LastName, e.DepartmentId, e.IsSuperVisor, 
                            d.Id, d.Name DepartmentName, d.Budget
                            FROM Employee e
                            JOIN Department d ON e.DepartmentId = d.Id
                        ";

            if (!EmployeeExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            }

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                   
                    

                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    

                    if (reader.Read())
                    {
                       
                     
                              Employee employee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor")),
                            department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                Name = reader.GetString(reader.GetOrdinal("DepartmentName")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                            }

                        };

          
                        reader.Close();

                        return Ok(employee);
                    };

                    return new StatusCodeResult(StatusCodes.Status404NotFound);
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
