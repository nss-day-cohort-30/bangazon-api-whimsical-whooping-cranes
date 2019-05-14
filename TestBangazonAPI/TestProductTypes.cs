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
    public class TestProductTypes
    {
        [Fact]
        public async Task Test_Get_All_ProductTypes()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/productTypes");

                string responseBody = await response.Content.ReadAsStringAsync();
                var productTypes = JsonConvert.DeserializeObject<List<ProductType>>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(productTypes.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_Single_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/productTypes/1");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var productType = JsonConvert.DeserializeObject<ProductType>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("James Farrel Parephernalia", productType.Name);
                Assert.NotNull(productType);
            }
        }

        [Fact]
        public async Task Test_Get_NonExistant_ProductType_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("/productTypes/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Create_And_Delete_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                ProductType grenades = new ProductType
                {
                    Name = "Grenades"
                };
                var grenadesAsJSON = JsonConvert.SerializeObject(grenades);

                var response = await client.PostAsync(
                    "/productTypes",
                    new StringContent(grenadesAsJSON, Encoding.UTF8, "application/json")
                );

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var newGrenades = JsonConvert.DeserializeObject<ProductType>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("Grenades", newGrenades.Name);

                // Delete the newly posted object

                var deleteResponse = await client.DeleteAsync($"/productTypes/{newGrenades.Id}");
                deleteResponse.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_NonExistent_ProductType_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                var deleteResponse = await client.DeleteAsync("/prodcuts/99999999");

                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Modify_ProductType()
        {
            // New quantity to change to and test
            string newName = "Chairs";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                 */
                ProductType modifiedProductType = new ProductType
                {
                    Name = newName
                };
                var modifiedProductTypeAsJSON = JsonConvert.SerializeObject(modifiedProductType);

                var response = await client.PutAsync(
                    "/productTypes/1002",
                    new StringContent(modifiedProductTypeAsJSON, Encoding.UTF8, "application/json")
                );
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                 */
                var getTestProductType = await client.GetAsync("/productTypes/1002");
                getTestProductType.EnsureSuccessStatusCode();

                string getTestProductTypeBody = await getTestProductType.Content.ReadAsStringAsync();
                ProductType newTestProductType = JsonConvert.DeserializeObject<ProductType>(getTestProductTypeBody);

                Assert.Equal(HttpStatusCode.OK, getTestProductType.StatusCode);
                Assert.Equal(newName, newTestProductType.Name);
            }
        }
    }
}