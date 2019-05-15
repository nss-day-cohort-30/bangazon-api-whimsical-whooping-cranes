using System;
using System.Collections.Generic;
using System.Data;
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
    public class CustomersController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CustomersController(IConfiguration config)
        {
            _config = config;
        }

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> GetCustomers(string pdq)
        {
            //This includes related products in the response
            string sql = @"SELECT
                        c.Id, c.FirstName, c.LastName
                        FROM Customer c 
                        WHERE 2=2";

            //this will neglect related product list
            if (pdq != null)
            {
                sql = $@"SELECT c.Id, c.FirstName, c.LastName, p.Title
                        FROM Customer c
                        left JOIN Product p ON c.Id = p.CustomerId";
            }
            {
                sql = $@"SELECT c.Id, c.FirstName, c.LastName
                        FROM Customer c";
            }


            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    if (pdq != null)
                    {
                        cmd.Parameters.Add(new SqlParameter("@p.Id", pdq));
                    }
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Dictionary<int, Customer> customerHashForProductsListed = new Dictionary<int, Customer>();
                    while (reader.Read())
                    {
                        int customerId = reader.GetInt32(reader.GetOrdinal("Id"));

                        if (!customerHashForProductsListed.ContainsKey(customerId))
                        {
                            customerHashForProductsListed[customerId] = new Customer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            };
                          
                            customerHashForProductsListed[customerId].CustomerProducts.Add(new Product
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"))
                            });
                        }
                    }

                    List<Customer> customers = customerHashForProductsListed.Values.ToList();
                    reader.Close();

                    return Ok(customers);
                }
            }
        }

        // GET specific customer information
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, string pdq)
        {
            if (!CustomerExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            }

            //This will get specific customer info AND related products if additional argument is passed in w/ the customer id.
            string sql = @"SELECT
                        c.Id, c.FirstName, c.LastName
                        FROM Customer c
                        WHERE 2=2";

            if (pdq != null)
            {
                sql = $@"SELECT c.Id, c.FirstName, c.LastName, p.Title
                        FROM Customer c
                         left JOIN Product p ON c.Id = p.CustomerId";
            }


            // add an IF not found error here
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sql;

                        cmd.CommandText = $@"SELECT c.Id, c.FirstName, c.LastName, p.Title, p.Id, p.ProductTypeId, p.Description, p.Quantity, p.CustomerId
                    FROM Customer c 
                    left JOIN Product p ON p.CustomerId = c.Id
                    WHERE @Id = c.id";

                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        SqlDataReader reader = await cmd.ExecuteReaderAsync();

                        Customer customer = null;

                        if (reader.Read())
                        {
                            customer = new Customer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            };
                            if (!reader.IsDBNull(reader.GetOrdinal("Title")))
                            {

                            customer.CustomerProducts.Add(new Product
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"))
                            });
                            }

                        }


                        reader.Close();

                        return Ok(customer);
                    }
                }
            }
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Customer customer)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())

                {
                    // More string interpolation
                    cmd.CommandText = @"
                        INSERT INTO Customer ()
                        OUTPUT INSERTED.Id
                        VALUES ()
                    ";
                    cmd.Parameters.Add(new SqlParameter("@firstName", customer.FirstName));

                    customer.Id = (int)await cmd.ExecuteScalarAsync();

                    return CreatedAtRoute("GetCustomer", new { id = customer.Id }, customer);
                }
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Customer customer)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE Customer
                            SET FirstName = @firstName
                            -- Set the remaining columns here
                            WHERE Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@id", customer.Id));
                        cmd.Parameters.Add(new SqlParameter("@firstName", customer.FirstName));

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
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]


        private bool CustomerExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = "SELECT Id FROM Customer WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}
