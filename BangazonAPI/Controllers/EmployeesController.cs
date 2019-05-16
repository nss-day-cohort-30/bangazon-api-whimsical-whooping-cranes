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

        //allows user to get all the computers from the database with their department and computer

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $@"SELECT * FROM Employee e 
                                            LEFT JOIN Department d ON e.DepartmentId = d.id 
                                            LEFT JOIN ComputerEmployee ce ON e.id = ce.EmployeeId 
                                            LEFT JOIN Computer c ON ce.ComputerId = c.Id";
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<Employee> employees = new List<Employee>();
                    while (reader.Read())
                    {
                        //if the ComputerEmployee joining table does not include EmployeeId then don't create the computer object instance
                        //just list the department because every employee is assigned to a department, but right now Katerina in the database in not yet assigned a computer 
                        if (reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                        {
                            Employee employee = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor")),
                                department = new Department
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                                }
                            };
                            employees.Add(employee);
                        }
                        else
                        {
                            if (reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                            {
                                Employee employee = new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                    IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor")),
                                    department = new Department
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                        Name = reader.GetString(reader.GetOrdinal("Name")),
                                        Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                                    },
                                    computer = new Computer
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                                        PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                        Make = reader.GetString(reader.GetOrdinal("Make")),
                                        Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                                    }
                                };
                                employees.Add(employee);
                            }
                            else
                            {
                                Employee employee = new Employee
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                    IsSuperVisor = reader.GetBoolean(reader.GetOrdinal("IsSuperVisor")),
                                    department = new Department
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                                        Name = reader.GetString(reader.GetOrdinal("Name")),
                                        Budget = reader.GetInt32(reader.GetOrdinal("Budget"))
                                    },
                                    computer = new Computer
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("ComputerId")),
                                        PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                                        DecomissionDate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate")),
                                        Make = reader.GetString(reader.GetOrdinal("Make")),
                                        Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer"))
                                    }
                                };
                                employees.Add(employee);
                            }
                        }
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
