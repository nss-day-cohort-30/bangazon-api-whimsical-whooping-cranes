using System;
using System.Net;
using Newtonsoft.Json;
using Xunit;
using BangazonAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace TestBangazonAPI
{
    public class TestProducts
    {
        [Fact]
        public async Task Test_Get_All_Products()
        {
            {

                using (var client = new APIClientProvider().Client)
                {
                    var response = await client.GetAsync("api/products");

                   

                    string responseBody = await response.Content.ReadAsStringAsync();
                    var productList = JsonConvert.DeserializeObject<List<Product>>(responseBody);

                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    Assert.True(productList.Count > 0);
                }
            }
        }

        [Fact]
        public async Task Test_Get_Single_Product()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/products/1");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<Product>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Happy-Shirt", product.Title);
                Assert.Equal("Tshirt", product.Description);
                Assert.NotNull(product);
            }
        }

        [Fact]
        public async Task Test_Create_Modify_And_Delete_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                Product test = new Product
                {
                   Title = "test",
                   Description = "testestst",
                   Price = 1431434,
                   Quantity = 2,
                   ProductTypeId = 1,
                   CustomerId = 1
                };
                var testAsJSON = JsonConvert.SerializeObject(test);


                var response = await client.PostAsync(
                    "/api/products",
                    new StringContent(testAsJSON, Encoding.UTF8, "application/json")
                );

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var newTest = JsonConvert.DeserializeObject<Product>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("test", newTest.Title);
                Assert.Equal("testestst", newTest.Description);
                Assert.Equal(1431434, newTest.Price);
                //////////////////////////////
                ///

                string newTitle = "NEWTESTTITLE";

                /*
                   PUT section
                */
                Product modifiedProduct = new Product
                {
                    Title = "NEWTESTTITLE",
                    Description = "testestst",
                    Price = 1431434,
                    Quantity = 2,
                    ProductTypeId = 1,
                    CustomerId = 1
                };
                var modifiedProductAsJSON = JsonConvert.SerializeObject(modifiedProduct);

                var Modifyresponse = await client.PutAsync(
                    $"/api/products/{newTest.Id}",
                    new StringContent(modifiedProductAsJSON, Encoding.UTF8, "application/json")
                );
                response.EnsureSuccessStatusCode();
                string ModifyresponseBody = await Modifyresponse.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, Modifyresponse.StatusCode);

                /*
                    GET section
                 */
                var getProduct = await client.GetAsync($"/api/products/{newTest.Id}");
                getProduct.EnsureSuccessStatusCode();

                string getProductBody = await getProduct.Content.ReadAsStringAsync();
                Product newProduct = JsonConvert.DeserializeObject<Product>(getProductBody);

                Assert.Equal(HttpStatusCode.OK, getProduct.StatusCode);
                Assert.Equal(newTitle, newProduct.Title);


                //DELETE TEST
                var deleteResponse = await client.DeleteAsync($"/api/products/{newProduct.Id}");
                deleteResponse.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_NonExistent_Product_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var deleteResponse = await client.DeleteAsync("/api/products/600000");

                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Get_NonExitant_Product_Fails()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/products/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
    }
}

