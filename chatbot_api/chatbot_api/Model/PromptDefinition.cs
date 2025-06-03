using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace chatbot_api.Model
{
    public class PromptDefinition
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("key")]
        public string Key { get; set; } = null!;

        [BsonElement("description")]
        public string Description { get; set; } = null!;

        [BsonElement("promptText")]
        public string PromptText { get; set; } = null!;

        [BsonElement("requiresApi")]
        public bool RequiresApi { get; set; } = false;

        [BsonElement("requiresGpt")]
        public bool RequiresGpt { get; set; } = true;

        [BsonElement("endpoint")]
        public PromptEndpoint? Endpoint { get; set; }
    }
}
