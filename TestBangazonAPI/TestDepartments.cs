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


        //This Test checks if the Http request to get a single department is successful.
        //If it fails, go to the DepartmentsController to see if the get single department request is still functioning properly.
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


        //This test checks two Http requests at once. It tests Post(creating) a new department Http request first. 
        //It then tests the Put(edit) Http request using the epartment it just created. If it fails, go to the 
        //Departments controller and depending on the error, check the Post or Put Http requests.
        [Fact]
        public async Task Test_Create_Modify_department()
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

                //////////////////////////////
                ///

                string newName = "testNEW";

                /*
                   PUT section
                */
                Department newNameTest = new Department
                {
                    Name = "testNEW",
                    Budget = 321
                };
                var modifiedDepartmentAsJSON = JsonConvert.SerializeObject(newNameTest);

                var Modifyresponse = await client.PutAsync(
                    $"/api/departments/{newTest.Id}",
                    new StringContent(modifiedDepartmentAsJSON, Encoding.UTF8, "application/json")
                );
                response.EnsureSuccessStatusCode();
                string ModifyresponseBody = await Modifyresponse.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, Modifyresponse.StatusCode);

                /*
                    GET section
                 */
                var getDepartment = await client.GetAsync($"/api/departments/{newTest.Id}");
                getDepartment.EnsureSuccessStatusCode();

                string getDepartmentBody = await getDepartment.Content.ReadAsStringAsync();
                Department newDepartment = JsonConvert.DeserializeObject<Department>(getDepartmentBody);

                Assert.Equal(HttpStatusCode.OK, getDepartment.StatusCode);
                Assert.Equal(newName, newDepartment.Name);


                //DELETE TEST
                // var deleteResponse = await client.DeleteAsync($"/api/departments/{newTest.Id}");
                // deleteResponse.EnsureSuccessStatusCode();
                // Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }


        

        //This test checks if the Get a nonexistent department will get a department that doesn't exist. If it
        //fails go the the get Single Departments Http request in the DepartmentsController and make sure it is still
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

