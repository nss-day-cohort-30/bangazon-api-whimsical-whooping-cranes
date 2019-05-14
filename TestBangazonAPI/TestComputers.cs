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
    public class TestComputers
    {
        [Fact]
        public async Task Test_Get_All_Computers()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/computers");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var computerList = JsonConvert.DeserializeObject<List<Computer>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(computerList.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_Single_Computer()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/computers/1");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var computer = JsonConvert.DeserializeObject<Computer>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Mojave", computer.Make);
                Assert.Equal("Apple", computer.Manufacturer);
                Assert.NotNull(computer);
            }
        }

        [Fact]
        public async Task Test_Get_NonExitant_Computer_Fails()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/computers/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }


        [Fact]
        public async Task Test_Create_And_Delete_Computer()
        {
            using (var client = new APIClientProvider().Client)
            {

                DateTime purchasedate = DateTime.Now;

                Computer macbookSuperPro = new Computer
                {
                    Make = "MacBook Super Pro",
                    Manufacturer = "Apple",
                    PurchaseDate = purchasedate,
                };
                var macbookSuperProAsJSON = JsonConvert.SerializeObject(macbookSuperPro);


                var response = await client.PostAsync(
                    "/computers",
                    new StringContent(macbookSuperProAsJSON, Encoding.UTF8, "application/json")
                );

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var newmacbookSuperPro = JsonConvert.DeserializeObject<Computer>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("MacBook Super Pro", newmacbookSuperPro.Make);
                Assert.Equal("Apple", newmacbookSuperPro.Manufacturer);
                Assert.Equal(purchasedate, newmacbookSuperPro.PurchaseDate);


                var deleteResponse = await client.DeleteAsync($"/computers/{newmacbookSuperPro.Id}");
                deleteResponse.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_NonExistent_Computer_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var deleteResponse = await client.DeleteAsync("/computers/600000");

                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Create_Modify_And_Delete_Computer()
        {

            DateTime purchasedate = DateTime
            using (var client = new APIClientProvider().Client)
            {
                Computer test = new Computer
                {
                  Make = "test",
                  Manufacturer = "test",
                  PurchaseDate = DateTime.Now,
                  DecomissionDate = DateTime.Now
                };
                var testAsJSON = JsonConvert.SerializeObject(test);


                var response = await client.PostAsync(
                    "/computers",
                    new StringContent(testAsJSON, Encoding.UTF8, "application/json")
                );

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var newTest = JsonConvert.DeserializeObject<Computer>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("test", newTest.Make);
                Assert.Equal("test", newTest.Manufacturer);
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
    }
}

