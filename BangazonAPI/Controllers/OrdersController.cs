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

/*
    Purpose: Controller for Orders
    Author: Mo Silvera
    Methods: Get single, Get all, Post, Put, and Delete, OrderExists
  */

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IConfiguration _config;

        public OrdersController(IConfiguration config)
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

        // GET ALL
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())

                {
                    cmd.CommandText = @"SELECT
                    o.Id, o.PaymentTypeId,
                    op.ProductId,
                    p.CustomerId, p.Price, p.Title, p.Description, p.ProductTypeId, p.Quantity,
                    pt.Name
                    FROM [Order] o
                    JOIN OrderProduct op ON op.OrderId = o.Id
                    JOIN Product p ON op.ProductId = p.Id
                    JOIN ProductType pt ON p.ProductTypeId = pt.Id;";

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Dictionary<int, Order> orderHash = new Dictionary<int, Order>();

                    while (reader.Read())
                    {
                        int orderId = reader.GetInt32(reader.GetOrdinal("Id"));

                        if (!orderHash.ContainsKey(orderId))
                        {
                            orderHash[orderId] = new Order
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                PaymentTypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId"))
                            };
                        }

                        orderHash[orderId].productsInOrder = new List<Product>();

                        orderHash[orderId].productsInOrder.Add(new Product
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("ProductId")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                            ProductType = new ProductType
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            }
                        });


                    }
                    List<Order> orders = orderHash.Values.ToList();

                    reader.Close();

                    return Ok(orders);
                }
            }
        }

        //GET ONE takes Id of desired PaymentType as an argument
        [HttpGet("{id}", Name = "GetOrder")]
        public async Task<IActionResult> Get(int id)
        {
            if (!OrderExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            }
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT
                    o.Id, o.PaymentTypeId,
                    op.ProductId,
                    p.CustomerId, p.Price, p.Title, p.Description, p.ProductTypeId, p.Quantity,
                    pt.Name
                    FROM [Order] o
                    JOIN OrderProduct op ON op.OrderId = o.Id
                    JOIN Product p ON op.ProductId = p.Id
                    JOIN ProductType pt ON p.ProductTypeId = pt.Id
                    WHERE @Id = o.id;";

                    cmd.Parameters.Add(new SqlParameter("@Id", id));

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Dictionary<int, Order> orderHash = new Dictionary<int, Order>();

                    while (reader.Read())
                    {
                        int orderId = reader.GetInt32(reader.GetOrdinal("Id"));

                        if (!orderHash.ContainsKey(orderId))
                        {
                            orderHash[orderId] = new Order
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                PaymentTypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId"))
                            };
                        }

                        orderHash[orderId].productsInOrder = new List<Product>();

                        orderHash[orderId].productsInOrder.Add(new Product
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("ProductId")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                            Title = reader.GetString(reader.GetOrdinal("Title")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                            ProductType = new ProductType
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            }
                        });


                    }
                    Order order = orderHash.Values.SingleOrDefault();

                    reader.Close();

                    return Ok(order);
                }
            }
        }

        // POST takes argument of type PaymentType 
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Order order)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO [Order] (CustomerId, PaymentTypeId)
                                        OUTPUT INSERTED.Id
                                        VALUES (@CustomerId, @PaymentTypeId)";
                    cmd.Parameters.Add(new SqlParameter("@PaymentTypeId", order.PaymentTypeId));
                    cmd.Parameters.Add(new SqlParameter("@CustomerId", order.CustomerId));


                    int newId = (int)await cmd.ExecuteScalarAsync();
                    order.Id = newId;
                    return CreatedAtRoute("GetOrder", new { id = newId }, order);
                }
            }
        }

        // PUT takes Id of the PaymentType you want to edit as first argument
        //second argument is the modified PaymentType object you want to PUT
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Order order)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $@"
                            UPDATE [Order]
                            SET CustomerId = @CustomerId,
                            PaymentTypeId = @PaymentTypeId,
                            WHERE Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@CustomerId", order.CustomerId));
                        cmd.Parameters.Add(new SqlParameter("@PaymentTypeId", order.PaymentTypeId));
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
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        //// DELETE takes Id of the PaymentType you want to delete as an argument
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete([FromRoute] int id)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = Connection)
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd = conn.CreateCommand())
        //            {
        //                cmd.CommandText = @"DELETE FROM PaymentType WHERE Id = @id";
        //                cmd.Parameters.Add(new SqlParameter("@id", id));

        //                int rowsAffected = await cmd.ExecuteNonQueryAsync();
        //                if (rowsAffected > 0)
        //                {
        //                    return new StatusCodeResult(StatusCodes.Status204NoContent);
        //                }
        //                throw new Exception("No rows affected");
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        if (!PaymentTypeExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //}

        ///*Boolean that will indicate if a payment type exists
        // * takes the Id of the payament type to check as an argument */
        private bool OrderExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id FROM [Order] WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }

        }
    }
}
