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
    public class TestTrainingPrograms
    {
        [Fact]
        public async Task Test_Get_All_TrainingPrograms()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/trainingPrograms");

                string responseBody = await response.Content.ReadAsStringAsync();
                var trainingPrograms = JsonConvert.DeserializeObject<List<TrainingProgram>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(trainingPrograms.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_Single_TrainingProgram()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/trainingPrograms/1");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var trainingProgram = JsonConvert.DeserializeObject<TrainingProgram>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("James Farrel Parephernalia", trainingProgram.Name);
                Assert.NotNull(trainingProgram);
            }
        }

        [Fact]
        public async Task Test_Get_NonExistant_TrainingProgram_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/trainingPrograms/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Create_And_Delete_TrainingProgram()
        {
            using (var client = new APIClientProvider().Client)
            {
                TrainingProgram grenades = new TrainingProgram
                {
                    Name = "Grenades"
                };
                var grenadesAsJSON = JsonConvert.SerializeObject(grenades);

                var response = await client.PostAsync(
                    "/traingingPrograms",
                    new StringContent(grenadesAsJSON, Encoding.UTF8, "application/json")
                );

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var newGrenades = JsonConvert.DeserializeObject<TrainingProgram>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Grenades", newGrenades.Name);

                // Delete the newly posted object

                var deleteResponse = await client.DeleteAsync($"/trainingPrograms/{newGrenades.Id}");
                deleteResponse.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_NonExistent_TrainingProgram_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var deleteResponse = await client.DeleteAsync("/prodcuts/99999999");

                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Modify_TrainingProgram()
        {
            // New quantity to change to and test
            string newName = "Chairs";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                 */
                TrainingProgram modifiedTrainingProgram = new TrainingProgram
                {
                    Name = newName
                };
                var modifiedTrainingProgramAsJSON = JsonConvert.SerializeObject(modifiedTrainingProgram);

                var response = await client.PutAsync(
                    "/trainingPrograms/1002",
                    new StringContent(modifiedTrainingProgramAsJSON, Encoding.UTF8, "application/json")
                );
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                 */
                var getTestTrainingProgram = await client.GetAsync("/trainingPrograms/1002");
                getTestTrainingProgram.EnsureSuccessStatusCode();

                string getTestTrainingProgramBody = await getTestTrainingProgram.Content.ReadAsStringAsync();
                TrainingProgram newTestTrainingProgram = JsonConvert.DeserializeObject<TrainingProgram>(getTestTrainingProgramBody);

                Assert.Equal(HttpStatusCode.OK, getTestTrainingProgram.StatusCode);
                Assert.Equal(newName, newTestTrainingProgram.Name);
            }
        }
    }
}