﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : Controller
    {
        private readonly IConfiguration _config;

        public DepartmentsController(IConfiguration config)
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
        public async Task<IActionResult> GetAllDepartments(string _include)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    

                    if (_include == "employees")
                    {
                        cmd.CommandText = $@"SELECT
                    e.Id as EmployeeId, e.FirstName, e.LastName, e.DepartmentId, e.IsSuperVisor, d.Id, d.Name, d.Budget
                    FROM Employee e
                    JOIN Department d ON e.DepartmentId = d.Id";
                    }
                    else
                    {
                        cmd.CommandText = @"SELECT Id, Name, Budget FROM Department";
                    }
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<Department> departments = new List<Department>();
                    while (reader.Read())
                    {
                        if (departments.Count < reader.GetInt32(reader.GetOrdinal("Id")))
                        {
                            Department department = new Department
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                            };

                            if (_include == "employees")
                            {
                                Employee employee = new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                    IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor"))
                                };

                                department.employees.Add(employee);
                            }
                            departments.Add(department);
                        }
                    }

                    reader.Close();

                    return Ok(departments);
                }
            }
        }

        [HttpGet("{id}", Name = "GetDepartment")]
        public async Task<IActionResult> Get(int id)
        {
            if (!DepartmentExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            }
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name, Budget
                                            FROM Department
                                           WHERE @id = Id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Department department = null;
                    if (reader.Read())
                    {
                       department = new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        };

                      };
                    

                    reader.Close();

                    return Ok(department);
                }
            }
        }


        // This Http request allows you to create a new product
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Department department)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Department(Name, Budget)
                                        OUTPUT INSERTED.Id
                                        VALUES (@Name, @Budget)";
                    cmd.Parameters.Add(new SqlParameter("@Name", department.Name));
                    cmd.Parameters.Add(new SqlParameter("@Budget", department.Budget));
                    int newId = (int)cmd.ExecuteScalar();
                    department.Id = newId;
                    return CreatedAtRoute("GetProduct", new { id = newId }, department);
                }
            }
        }




        private bool DepartmentExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                       SELECT Id, Name, Budget                                            
                       FROM Department
                       WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}