using System;
using System.Net;
using Newtonsoft.Json;
using Xunit;
using BangazonAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

/*  Purpose: Test for PaymentTypes
    Author: Mo Silvera
    Methods: Test get single, get all, test post/put/delete, test get non existant
  */

namespace TestBangazonAPI
{
    public class TestPaymentType
    {
        [Fact]
        public async Task Test_Get_All_Payment_Types()
        {
          
            
                using (var client = new APIClientProvider().Client)
                {
                    var response = await client.GetAsync("api/PaymentTypes");

                    

                    string responseBody = await response.Content.ReadAsStringAsync();
                    var paymentTypeList = JsonConvert.DeserializeObject<List<PaymentType>>(responseBody);

                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    Assert.True(paymentTypeList.Count > 0);
                }

            
        }
        //Tests Get Single method on Payment Type controller
        
        [Fact]
        public async Task Test_Get_Single_Payment_Type()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/PaymentTypes/1");

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var paymentType = JsonConvert.DeserializeObject<PaymentType>(responseBody);

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Johns BigBank", paymentType.Name);
                Assert.Equal(1, paymentType.CustomerId);
                Assert.Equal(1, paymentType.AcctNumber);
                Assert.NotNull(paymentType);
            }
        }

        //Tests Post, Put, Delete Methods on Payment Type Controller

        [Fact]
        public async Task Test_Create_And_Modify_And_Delete_Payment_Type()
        {
            using (var client = new APIClientProvider().Client)
            {
                //POST
                PaymentType testPaymentType = new PaymentType
                {
                    AcctNumber = 1111,
                    Name = "Test Payment Name",
                    CustomerId = 1
                  
                };
                var testPaymentTypeAsJSON = JsonConvert.SerializeObject(testPaymentType);


                var response = await client.PostAsync(
                    "api/PaymentTypes",
                    new StringContent(testPaymentTypeAsJSON, Encoding.UTF8, "application/json")
                );

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var newTestPaymentType = JsonConvert.DeserializeObject<PaymentType>(responseBody);

                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(1111, newTestPaymentType.AcctNumber);
                Assert.Equal("Test Payment Name", newTestPaymentType.Name);
                Assert.Equal(1, newTestPaymentType.CustomerId);

                //PUT
                int newAcctNumber = 1234567;

                PaymentType modifiedPaymentType = new PaymentType
                {
                    AcctNumber = newAcctNumber,
                    Name = "Johns BigBank",
                    CustomerId = 1
                };
                var modifiedPaymentTypeJSON = JsonConvert.SerializeObject(modifiedPaymentType);

                var modifiedResponse = await client.PutAsync(
                    $"api/PaymentTypes/{newTestPaymentType.Id}",
                    new StringContent(modifiedPaymentTypeJSON, Encoding.UTF8, "application/json")
                );
                modifiedResponse.EnsureSuccessStatusCode();
                string modifiedResponseBody = await modifiedResponse.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, modifiedResponse.StatusCode);

                var getPaymentType = await client.GetAsync($"api/PaymentTypes/{newTestPaymentType.Id}");
                getPaymentType.EnsureSuccessStatusCode();

                string getPaymentTypeBody = await getPaymentType.Content.ReadAsStringAsync();
                PaymentType newPaymentType = JsonConvert.DeserializeObject<PaymentType>(getPaymentTypeBody);

                Assert.Equal(HttpStatusCode.OK, getPaymentType.StatusCode);
                Assert.Equal(newAcctNumber, newPaymentType.AcctNumber);

                //DELETE 
                var deleteResponse = await client.DeleteAsync($"api/PaymentTypes/{newTestPaymentType.Id}");
                deleteResponse.EnsureSuccessStatusCode();
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        
    
    //Tests 404 response received when asking for non-existant payment type
    [Fact]
        public async Task Test_Get_NonExistant_Payment_Type_Fails()
        {

            using (var client = new APIClientProvider().Client)
            {
                var response = await client.GetAsync("api/PaymentTypes/999999999");
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
    }
}
