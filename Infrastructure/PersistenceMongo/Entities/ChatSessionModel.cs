using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PersistenceMongo.Entities;

public class ChatSessionModel
{
    [BsonId] 
    [BsonRepresentation(BsonType.ObjectId)] 
    public string? Id { get; set; }

    public int? UserId { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.Now;

    public List<ChatHistoryModel> History { get; set; }=new List<ChatHistoryModel>();
}