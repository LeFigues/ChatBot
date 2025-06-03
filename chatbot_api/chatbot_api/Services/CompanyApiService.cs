namespace chatbot_api.Services
{
    public class CompanyApiService
    {
        private readonly HttpClient _client;

        public CompanyApiService(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("CompanyAPI");
        }

        public async Task<string> GetStocksAsync()
        {
            var response = await _client.GetAsync("api/Stocks");
            if (!response.IsSuccessStatusCode) return "[]";
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetProductsAsync()
        {
            var response = await _client.GetAsync("api/Products");
            if (!response.IsSuccessStatusCode) return "[]";
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetBrandsAsync()
        {
            var response = await _client.GetAsync("api/Brands");
            if (!response.IsSuccessStatusCode) return "[]";
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetCategoriesAsync()
        {
            var response = await _client.GetAsync("api/Categories");
            if (!response.IsSuccessStatusCode) return "[]";
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetBranchesAsync()
        {
            var response = await _client.GetAsync("api/Branches");
            if (!response.IsSuccessStatusCode) return "[]";
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetProductByIdAsync(string id)
        {
            var response = await _client.GetAsync($"api/products/{id}");
            if (!response.IsSuccessStatusCode) return "{}";
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetJsonFromEndpoint(string relativeUrl)
        {
            var response = await _client.GetAsync(relativeUrl);
            if (!response.IsSuccessStatusCode) return "[]";
            return await response.Content.ReadAsStringAsync();
        }

    }
}
