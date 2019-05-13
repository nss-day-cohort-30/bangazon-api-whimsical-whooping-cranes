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
        public async Task Test_Modify_Computer()
        {
            // New last name to change to and test
            string newMake = "Macbook Pro 3000";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                 */
                Computer modifiedMacBook = new Computer
                {
                    Make = newMake,
                    Manufacturer = "Apple",
                };
                var modifiedKateAsJSON = JsonConvert.SerializeObject(modifiedMacBook);

                var response = await client.PutAsync(
                    "/computers/1",
                    new StringContent(modifiedKateAsJSON, Encoding.UTF8, "application/json")
                );
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                 */
                var getMacBook = await client.GetAsync("/computers/1");
                getMacBook.EnsureSuccessStatusCode();

                string getMacBody = await getMacBook.Content.ReadAsStringAsync();
                Computer newMacBook = JsonConvert.DeserializeObject<Computer>(getMacBody);

                Assert.Equal(HttpStatusCode.OK, getMacBook.StatusCode);
                Assert.Equal(newMake, newMacBook.Make);
            }
        }
    }
}

