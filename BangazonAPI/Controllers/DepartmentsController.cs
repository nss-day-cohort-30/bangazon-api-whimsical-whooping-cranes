using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonAPI.Controllers
{
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
        public async Task<IActionResult> GetAllDepartments(string _includes )
        {
            string sql = @"SELECT d.Id, d.Name, d.Budget,
                        e.Id, e.FirstName, e.LastName, e.DepartmentId
                        FROM Department d
                        JOIN Employee e ON e.DepartmentId = d.Id
                        WHERE 2=2
                        ";

            if (_includes != null)
            {
                sql = $"{sql} AND e.Id = @employeeId";
            }


            //if (exercise != null)
            //{
            //    sql = $"{sql} AND e.Name LIKE @exerciseName";
            //}

            if (q != null)
            {
                sql = $@"{sql} AND (
                    s.LastName LIKE @q
                    OR s.FirstName LIKE @q
                    OR s.SlackHandle LIKE @q
                    )
                    ";

            }

            if (orderBy != null)
            {
                sql = $"{sql} ORDER BY {orderBy}";
            }

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    if (cohort != null)
                    {
                        cmd.Parameters.Add(new SqlParameter("@cohortId", cohort));
                    }
                    if (q != null)
                    {
                        cmd.Parameters.Add(new SqlParameter("@q", $"%{q}%"));

                    }
                    if (exercise != null)
                    {
                        cmd.Parameters.Add(new SqlParameter("@exerciseName", $"%{exercise}%"));
                    }

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Dictionary<int, Student> studentHash = new Dictionary<int, Student>();

                    while (reader.Read())
                    {
                        int studentId = reader.GetInt32(reader.GetOrdinal("Id"));

                        if (!studentHash.ContainsKey(studentId))
                        {
                            studentHash[studentId] = new Student
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                SlackHandle = reader.GetString(reader.GetOrdinal("SlackHandle")),
                                Cohort = new Cohort
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                    Name = reader.GetString(reader.GetOrdinal("CohortName"))
                                }
                            };
                        }

                        studentHash[studentId].AssignedExercises.Add(new Exercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("ExerciseId")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Language = reader.GetString(reader.GetOrdinal("Language"))
                        });


                    }
                    List<Student> students = studentHash.Values.ToList();
                    reader.Close();

                    return Ok(students);
                }
            }
        }
    }
}