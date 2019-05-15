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
        //Purpose: Test class for Product Controller
        //Author: Katerina Freeman
        //Methods: Testing get all products, get single product,
        //create edit and delete product, delete nonexistent product, and get noneistent product. 

        //This Test will check if the Get all products Http request is working. 
        //If it fails, go to the ProductsController to make sure it is still functioning properly. 
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


        //This Test checks if the Http request to get a single product is successful.
        //If it fails, go to the ProductsController to see if the get single product request is still functioning properly.
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


        //This test checks three Http requests at once. It tests Post(creating) a new product Http request first. 
        //It then tests the Put(edit) Http request using the product it just created. Finally it checks the delete
        // Http request to delete a product by deleting the new product we just modified. If it fails, go to the 
        //Products controller and depending on the error, check the Post, Put, or Delete Http requests.
        [Fact]
        public async Task Test_Create_Modify_And_Delete_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                //POST section
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


        //This test checks if the Delete Http request will delete a product that does not exist. If it fails, 
        //go to the productsController and check the Delete Http request.
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

        //This test checks if the Get a nonexistent product will get a product that doesn't exist. If it
        //fails go the the get Single Products Http request in ProductsController andmake sure it is still
        //functioning properly.

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

