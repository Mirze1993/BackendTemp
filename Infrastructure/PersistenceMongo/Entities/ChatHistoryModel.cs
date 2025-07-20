using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PersistenceMongo.Entities;

public class ChatHistoryModel
{
    [BsonId] 
    [BsonRepresentation(BsonType.ObjectId)] 
    public string? Id { get; set; }

    public string? Role   { get; set; }
    public string? Content   { get; set; }
    public string? AuthorName  { get; set; }
}