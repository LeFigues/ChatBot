using chatbot_api.Configurations;
using chatbot_api.Model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace chatbot_api.Services
{
    public class PromptService
    {
        private readonly IMongoCollection<PromptDefinition> _collection;

        public PromptService(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _collection = database.GetCollection<PromptDefinition>("Prompts");
        }

        public async Task<List<PromptDefinition>> GetAllAsync() =>
            await _collection.Find(_ => true).ToListAsync();

        public async Task<PromptDefinition?> GetByKeyAsync(string key) =>
            await _collection.Find(p => p.Key == key).FirstOrDefaultAsync();

        public async Task CreateAsync(PromptDefinition prompt) =>
            await _collection.InsertOneAsync(prompt);

        public async Task UpdateAsync(string key, PromptDefinition updated) =>
            await _collection.ReplaceOneAsync(p => p.Key == key, updated);

        public async Task DeleteAsync(string key) =>
            await _collection.DeleteOneAsync(p => p.Key == key);
    }
}
