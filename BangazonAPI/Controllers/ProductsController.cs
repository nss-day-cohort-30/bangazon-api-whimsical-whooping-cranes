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
    //Purpose: Product Controller
    //Author: Katerina Freeman
    //Methods: Get all products, get single product, post new product, edit existing product, 
    //delete existing product, and check if product exists. 


    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ProductsController(IConfiguration config)
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
        // this HTTP Request gets all Products while joining Customer and ProductType so that you can
        // see the customer who created said product and the producttype. 
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT p.Id, p.Price, p.Title, p.Description, p.Quantity,
                                        p.CustomerId, p.ProductTypeId, c.FirstName, c.LastName, t.Name
                                         FROM Product p 
                                         JOIN Customer c ON p.CustomerId = c.id
                                         JOIN ProductType t ON p.ProductTypeId = t.id";
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<Product> products = new List<Product>();
                    while (reader.Read())
                    {
                       Product product = new Product
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                            Customer = new Customer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName"))
                            },
                            ProductType = new ProductType
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            }




                        };

                        products.Add(product);
                    }

                    reader.Close();

                    return Ok(products);
                }
            }
        }

         //GET api/values/5
         //This HTTP request gets a single product by the argument Id. An example url to pull back one product is
         // Http://localhost:5000/api/products/1
         // 
        [HttpGet("{id}", Name = "GetProduct") ]
        public async Task<IActionResult> Get(int id)
        {
            if (!ProductExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            }
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT p.Id, p.Price, p.Title, p.Description, p.Quantity,
                                        p.CustomerId, p.ProductTypeId, c.FirstName, c.LastName, t.Name
                                         FROM Product p 
                                         JOIN Customer c ON p.CustomerId = c.id
                                         JOIN ProductType t ON p.ProductTypeId = t.id
                                           WHERE @id = p.Id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                   Product product = null;
                    if (reader.Read())
                    {
                         product = new Product
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                             CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                             ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                             Customer = new Customer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName"))
                            },
                            ProductType = new ProductType
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            }




                        };
                    }

                    reader.Close();

                    return Ok(product);
                }
            }
        }
        // This Http request allows you to create a new product
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Product product)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Product (Price, Title, Description, Quantity, CustomerId, ProductTypeId)
                                        OUTPUT INSERTED.Id
                                        VALUES (@Price, @Title, @Description, @Quantity, @CustomerId, @ProductTypeId)";
                    cmd.Parameters.Add(new SqlParameter("@Price", product.Price));
                    cmd.Parameters.Add(new SqlParameter("@Title", product.Title));
                    cmd.Parameters.Add(new SqlParameter("@Description", product.Description));
                    cmd.Parameters.Add(new SqlParameter("@Quantity", product.Quantity));
                    cmd.Parameters.Add(new SqlParameter("@CustomerId", product.CustomerId));
                    cmd.Parameters.Add(new SqlParameter("@ProductTypeId", product.ProductTypeId));
                    int newId = (int)cmd.ExecuteScalar();
                    product.Id = newId;
                    return CreatedAtRoute("GetProduct", new { id = newId }, product);
                }
            }
        }

        //This Http request allows you to edit an existing product. It takes the argument for a single Id.
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Product product)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Product
                                            SET Title = @Title,
                                                Price = @Price,
                                                Description = @Description,
                                                Quantity = @Quantity,
                                                CustomerId = @CustomerId,
                                                ProductTypeId = @ProductTypeId
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@Title", product.Title));
                        cmd.Parameters.Add(new SqlParameter("@Price", product.Price));
                        cmd.Parameters.Add(new SqlParameter("@Description", product.Description));
                        cmd.Parameters.Add(new SqlParameter("@Quantity",product.Quantity));
                        cmd.Parameters.Add(new SqlParameter("@CustomerId", product.CustomerId));
                        cmd.Parameters.Add(new SqlParameter("@ProductTypeId", product.ProductTypeId));
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
            catch (Exception)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        //This Http request allows you to delete a specific product from the database. It takes the argument of a single Id.
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
                        cmd.CommandText = @"DELETE FROM Product WHERE Id = @id";
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
            catch (Exception)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // This function checks if a product exists in the database by checking against it's Id. 
        private bool ProductExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                       SELECT Id,  Price, Title, Description,Quantity,CustomerId, ProductTypeId                                            
                       FROM Product
                       WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
    }
