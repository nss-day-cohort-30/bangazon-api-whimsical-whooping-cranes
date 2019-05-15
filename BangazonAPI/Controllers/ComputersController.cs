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
    Purpose: Controller for Computer Class
    Author: Abbey Brown
    Methods: Get single, Get all, Post, Put, and Delete

 */
namespace BangazonAPI.Controllers
{


    [Route("[controller]")]
    [ApiController]
    public class ComputersController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ComputersController(IConfiguration config)
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
        public async Task<IActionResult> GetComputers()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Make, Manufacturer, PurchaseDate, DecomissionDate FROM Computer";
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    List<Computer> computers = new List<Computer>();

                    while (reader.Read())
                    {
                        DateTime? decomissiondate = null;

                        if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate"))) {


                             decomissiondate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate"));
                        }


                        Computer computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            DecomissionDate = decomissiondate,

                        };

                        computers.Add(computer);
                    }
                    reader.Close();

                    return Ok(computers);
                }
            }
        }


        //allows user to get single computer from the database

        [HttpGet("{id}", Name = "GetComputer")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            if (!ComputerExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            }
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT
                            Id, Make, Manufacturer, PurchaseDate, DecomissionDate
                        FROM Computer
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Computer computer = null;

                    if (reader.Read())
                    {
                        DateTime? decomissiondate = null;

                        if (!reader.IsDBNull(reader.GetOrdinal("DecomissionDate")))
                        {


                            decomissiondate = reader.GetDateTime(reader.GetOrdinal("DecomissionDate"));
                        }

                        computer = new Computer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Make = reader.GetString(reader.GetOrdinal("Make")),
                            Manufacturer = reader.GetString(reader.GetOrdinal("Manufacturer")),
                            PurchaseDate = reader.GetDateTime(reader.GetOrdinal("PurchaseDate")),
                            DecomissionDate = decomissiondate,
                        };
                    }
                    reader.Close();

                    return Ok(computer);
                }
            }
        }
   
      
        //allows user to post to the database
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Computer computer)
        {

            string sql = @"INSERT INTO Computer (Make, Manufacturer, PurchaseDate)
                                        OUTPUT INSERTED.Id
                                        VALUES (@make, @manufacturer, @purchasedate)";

            using (SqlConnection conn = Connection)
            {
                conn.Open();

                if (computer.DecomissionDate == null)
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.Parameters.Add(new SqlParameter("@make", computer.Make));
                        cmd.Parameters.Add(new SqlParameter("@manufacturer", computer.Manufacturer ?? ""));
                        cmd.Parameters.Add(new SqlParameter("@purchasedate", computer.PurchaseDate));

                        int newId = (int)await cmd.ExecuteScalarAsync();
                        computer.Id = newId;
                        return CreatedAtRoute("GetComputer", new { id = newId }, computer);

                    }
                }

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Computer (Make, Manufacturer, PurchaseDate, DecomissionDate)
                                        OUTPUT INSERTED.Id
                                        VALUES (@make, @manufacturer, @purchasedate, @decomissiondate)";
                    cmd.Parameters.Add(new SqlParameter("@make", computer.Make));
                    cmd.Parameters.Add(new SqlParameter("@manufacturer", computer.Manufacturer ?? ""));
                    cmd.Parameters.Add(new SqlParameter("@purchasedate", computer.PurchaseDate));
                    cmd.Parameters.Add(new SqlParameter("@decomissiondate", computer.DecomissionDate));

                    int newId = (int)await cmd.ExecuteScalarAsync();
                    computer.Id = newId;
                    return CreatedAtRoute("GetComputer", new { id = newId }, computer);
                }
            }
        }


        //allows user to edit an object in the database (PUT method)

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Computer computer)
        {

            string sql = @"UPDATE Computer 
                                        SET Make = @make, 
                                            Manufacturer = @manufacturer, 
                                            PurchaseDate = @purchasedate
                                        WHERE Id = @id";

            using (SqlConnection conn = Connection)
            {
                conn.Open();

                if (computer.DecomissionDate == null)
                {
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;
                        cmd.Parameters.Add(new SqlParameter("@make", computer.Make));
                        cmd.Parameters.Add(new SqlParameter("@manufacturer", computer.Manufacturer));
                        cmd.Parameters.Add(new SqlParameter("@purchasedate", computer.PurchaseDate));
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");

                    }
                }

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Computer 
                                        SET Make = @make, 
                                            Manufacturer = @manufacturer, 
                                            PurchaseDate = @purchasedate, 
                                            DecomissionDate = @decomissiondate
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@make", computer.Make));
                    cmd.Parameters.Add(new SqlParameter("@manufacturer", computer.Manufacturer));
                    cmd.Parameters.Add(new SqlParameter("@purchasedate", computer.PurchaseDate));
                    cmd.Parameters.Add(new SqlParameter("@decomissiondate", computer.DecomissionDate));
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        return new StatusCodeResult(StatusCodes.Status204NoContent);
                    }
                    throw new Exception("No rows affected");
                }
            }
        }


        //allows user to delete object from the database (Delete Method)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Computer WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!ComputerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        //boolean to check and see if the computer exists in the database
        //this method takes the argument of the computer id that is the primary key implemented by each method
        private bool ComputerExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Make, Manufacturer, PurchaseDate, DecomissionDate
                        FROM Computer
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }

    }
}
