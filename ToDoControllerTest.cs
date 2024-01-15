using Microsoft.AspNetCore.Identity.Data;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using ToDoListApp.Models;

namespace ToDoIntegrationTest
{
    public class ToDoControllerTest
    {
        private readonly HttpClient _client;
        public ToDoControllerTest() 
        {
            _client = SetupClientAsync().Result;
        }

        private async Task<HttpClient> SetupClientAsync()
        {
            var application = new ToDoWebApplicationFactory();
            var client = application.CreateClient();

            var loginRequest = new LoginRequestModel
            {
                Username = "zulfanfahreza",
                Password = "zulfan12345",
            };
            var loginResponse = await client.PostAsJsonAsync("api/v1/Login", loginRequest);
            loginResponse.EnsureSuccessStatusCode();
            var token = loginResponse.Content.ReadAsStringAsync().Result;

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        [Fact]
        public async Task GetAllItemWithoutAddingItemTest()
        {
            var response = await _client.GetAsync("api/v1/ToDo");

            response.EnsureSuccessStatusCode();
            var responseResult = response.Content.ReadAsStringAsync().Result;
            var responseResultJson = JsonConvert.DeserializeObject<ItemCollectionResponseModel>(responseResult);
            Assert.Equal(1, responseResultJson.Items.Count());
        }

        [Fact]
        public async Task GetAllItemAfterAddingItemTest()
        {
            var addItemRequest = new AddUpdateItemRequestModel
            {
                Name = "Create REST API",
                IsComplete = true
            };
            var addItemResponse = await _client.PostAsJsonAsync("api/v1/ToDo", addItemRequest);
            addItemResponse.EnsureSuccessStatusCode();

            addItemRequest = new AddUpdateItemRequestModel
            {
                Name = "Add JWT authentication",
                IsComplete = true
            };
            addItemResponse = await _client.PostAsJsonAsync("api/v1/ToDo", addItemRequest);
            addItemResponse.EnsureSuccessStatusCode();

            var getAllItemResponse = await _client.GetAsync("api/v1/ToDo");

            getAllItemResponse.EnsureSuccessStatusCode();
            var getAllItemResponseResult = getAllItemResponse.Content.ReadAsStringAsync().Result;
            var getAllItemResponseResultJson = JsonConvert.DeserializeObject<ItemCollectionResponseModel>(getAllItemResponseResult);
            Assert.Equal(2, getAllItemResponseResultJson.Items.Count());

            var getItemByIdResponse = await _client.GetAsync("api/v1/ToDo/1");

            getItemByIdResponse.EnsureSuccessStatusCode();
            var getItemByIdResponseResult = getItemByIdResponse.Content.ReadAsStringAsync().Result;
            var getItemByIdResponseResultJson = JsonConvert.DeserializeObject<ToDoItemModel>(getItemByIdResponseResult);
            Assert.Equal("Create REST API", getItemByIdResponseResultJson.Name);
        }

        [Fact]
        public async Task UpdateItemTest()
        {
            var addItemRequest = new AddUpdateItemRequestModel
            {
                Name = "Create REST API",
                IsComplete = true
            };
            var addItemResponse = await _client.PostAsJsonAsync("api/v1/ToDo", addItemRequest);
            addItemResponse.EnsureSuccessStatusCode();

            var updateItemRequest = new AddUpdateItemRequestModel
            {
                Name = "Add unit testing",
                IsComplete = false
            };
            var updateItemResponse = await _client.PutAsJsonAsync("api/v1/ToDo/1", updateItemRequest);
            updateItemResponse.EnsureSuccessStatusCode();

            var getItemByIdResponse = await _client.GetAsync("api/v1/ToDo/1");

            getItemByIdResponse.EnsureSuccessStatusCode();
            var getItemByIdResponseResult = getItemByIdResponse.Content.ReadAsStringAsync().Result;
            var getItemByIdResponseResultJson = JsonConvert.DeserializeObject<ToDoItemModel>(getItemByIdResponseResult);
            Assert.Equal("Add unit testing", getItemByIdResponseResultJson.Name);
        }

        [Fact]
        public async Task DeleteItemTest()
        {
            var addItemRequest = new AddUpdateItemRequestModel
            {
                Name = "Create REST API",
                IsComplete = true
            };
            var addItemResponse = await _client.PostAsJsonAsync("api/v1/ToDo", addItemRequest);
            addItemResponse.EnsureSuccessStatusCode();

            addItemRequest = new AddUpdateItemRequestModel
            {
                Name = "Add JWT authentication",
                IsComplete = true
            };
            addItemResponse = await _client.PostAsJsonAsync("api/v1/ToDo", addItemRequest);
            addItemResponse.EnsureSuccessStatusCode();

            var deleteItemResponse = await _client.DeleteAsync("api/v1/ToDo/1");
            deleteItemResponse.EnsureSuccessStatusCode();

            var getAllItemResponse = await _client.GetAsync("api/v1/ToDo");

            getAllItemResponse.EnsureSuccessStatusCode();
            var getAllItemResponseResult = getAllItemResponse.Content.ReadAsStringAsync().Result;
            var getAllItemResponseResultJson = JsonConvert.DeserializeObject<ItemCollectionResponseModel>(getAllItemResponseResult);
            Assert.Equal(1, getAllItemResponseResultJson.Items.Count());
        }
    }
}