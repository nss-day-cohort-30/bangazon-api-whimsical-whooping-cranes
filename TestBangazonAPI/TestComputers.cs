using System;
using System.Net;
using Newtonsoft.Json;
using Xunit;
using BangazonAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

/*
    Purpose: Testing Class for Computer 
    Author: Abbey Brown
    Methods: Testing Get All, Get Single, Post, Put and Delete methods and checks for non-existant computers for 
    //get single and delete methods

 */
namespace TestBangazonAPI
{
    public class TestComputers
    {

      
        //test to get all computers from the database
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

        //test to get single computer from the database

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
                Assert.NotNull(computer);
            }
        }


        //tests for a not found if the computer does not exist in the database if user is trying to get single computer
        [Fact]
        public async Task Test_Get_NonExitant_Computer_Fails()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/computers/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }


        //test that creates and deletes a computer from the database

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



        //tests for a not found if the computer does not exist in the database if user is trying to delete
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

        //test that creates, modifies, and deletes a computer
        [Fact]
        public async Task Test_Create_Modify_And_Delete_Computer()
        {

            using (var client = new APIClientProvider().Client)
            {
                DateTime purchasedate = DateTime.Now;

                DateTime decomissiondate = DateTime.Now;

                Computer macbookSuperPro = new Computer
                {
                    Make = "MacBook Super Pro",
                    Manufacturer = "Apple",
                    PurchaseDate = purchasedate,
                    DecomissionDate = decomissiondate
                };

                var macbookSuperProAsJSON = JsonConvert.SerializeObject(macbookSuperPro);


                var response = await client.PostAsync(
                    "/computers",
                    new StringContent(macbookSuperProAsJSON, Encoding.UTF8, "application/json")
                );

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var newmacbookSuperPro = JsonConvert.DeserializeObject<Computer>(responseBody);

                string newMake = "test number two";

                /*
                   PUT section
                */
                Computer modifiedComputer = new Computer
                {
                    Make = newMake,
                    Manufacturer = "test",
                    PurchaseDate = purchasedate,
                    DecomissionDate = decomissiondate

                };
                var modifiedComputerAsJSON = JsonConvert.SerializeObject(modifiedComputer);

                int modifiedNewComputerId = newmacbookSuperPro.Id;

                var Modifyresponse = await client.PutAsync(
                    $"/computers/{modifiedNewComputerId}",
                    new StringContent(modifiedComputerAsJSON, Encoding.UTF8, "application/json")
                );

                Modifyresponse.EnsureSuccessStatusCode();
                string ModifyresponseBody = await Modifyresponse.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, Modifyresponse.StatusCode);

                /*
                    GET section
                 */
                var getComputer = await client.GetAsync($"/computers/{modifiedNewComputerId}");
                getComputer.EnsureSuccessStatusCode();

                string getComputerBody = await getComputer.Content.ReadAsStringAsync();
                Computer newComputer = JsonConvert.DeserializeObject<Computer>(getComputerBody);

                Assert.Equal(HttpStatusCode.OK, getComputer.StatusCode);
                Assert.Equal(newMake, newComputer.Make);


                //DELETE TEST
                var deleteResponse = await client.DeleteAsync($"/computers/{modifiedNewComputerId}");
                deleteResponse.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }
    }
}

