using MongoDB.Bson.Serialization.Attributes;

namespace chatbot_api.Model
{
    public class PromptEndpoint
    {
        [BsonElement("method")]
        public string Method { get; set; } = "GET";

        [BsonElement("url")]
        public string Url { get; set; } = null!;

        [BsonElement("params")]
        public List<string> Params { get; set; } = new();
    }
}
