using System;
using System.Net;
using Newtonsoft.Json;
using Xunit;
using BangazonAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TestBangazonAPI
{
    public class TestCustomers
    {
        [Fact]
        public async Task Test_Get_All_Customers()
        {
            using (var client = new APIClientProvider().Client)
            {
               var response = await client.GetAsync("/api/customers");

                string responseBody = await response.Content.ReadAsStringAsync();
                var customers = JsonConvert.DeserializeObject<List<Customer>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(customers.Count > 0);
            }
        }
        [Fact]
        public async Task Test_Get_Specific_Customer()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/customers/1");

                response.EnsureSuccessStatusCode();
                
                string responseBody = await response.Content.ReadAsStringAsync();
                var customer = JsonConvert.DeserializeObject<Customer>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Young", customer.FirstName);
                Assert.Equal("Thug", customer.LastName);
            }
        }
    }
}
