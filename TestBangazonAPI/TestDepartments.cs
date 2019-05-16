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
    public class TestDepartments
    {
         
        [Fact]
        public async Task Test_Get_All_Departments()
        {
            {

                using (var client = new APIClientProvider().Client)
                {
                    var response = await client.GetAsync("api/departments");

                   

                    string responseBody = await response.Content.ReadAsStringAsync();
                    var departmentList = JsonConvert.DeserializeObject<List<Department>>(responseBody);

                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    Assert.True(departmentList.Count > 0);
                }
            }
        }


        //This Test checks if the Http request to get a single product is successful.
        //If it fails, go to the ProductsController to see if the get single product request is still functioning properly.
        [Fact]
        public async Task Test_Get_Single_Department()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/departments/1");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var department = JsonConvert.DeserializeObject<Department>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Honky Horns", department.Name);
                Assert.Equal(300, department.Budget);
                Assert.NotNull(department);
            }
        }


        //This test checks three Http requests at once. It tests Post(creating) a new product Http request first. 
        //It then tests the Put(edit) Http request using the product it just created. Finally it checks the delete
        // Http request to delete a product by deleting the new product we just modified. If it fails, go to the 
        //Products controller and depending on the error, check the Post, Put, or Delete Http requests.
        [Fact]
        public async Task Test_Create_Modify_And_Delete_department()
        {
            using (var client = new APIClientProvider().Client)
            {
                //POST section
                Department test = new Department
                {
                   Name = "test",
                   Budget = 321
                   
                };
                var testAsJSON = JsonConvert.SerializeObject(test);


                var response = await client.PostAsync(
                    "/api/departments",
                    new StringContent(testAsJSON, Encoding.UTF8, "application/json")
                );

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var newTest = JsonConvert.DeserializeObject<Department>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("test", newTest.Name);
                Assert.Equal(321, newTest.Budget);

                
               
                //DELETE TEST
               // var deleteResponse = await client.DeleteAsync($"/api/departments/{newTest.Id}");
               // deleteResponse.EnsureSuccessStatusCode();
               // Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }


        

        //This test checks if the Get a nonexistent product will get a product that doesn't exist. If it
        //fails go the the get Single Products Http request in ProductsController andmake sure it is still
        //functioning properly.

        [Fact]
        public async Task Test_Get_NonExistant_Departments_Fails()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/api/departments/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
    }
}

