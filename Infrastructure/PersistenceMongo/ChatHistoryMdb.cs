using Domain.Request.AiChatHistory;
using MongoDB.Bson;
using MongoDB.Driver;
using PersistenceMongo.Entities;

namespace PersistenceMongo;

public class ChatHistoryMdb
{
    private readonly IMongoCollection<ChatSessionModel>  _collection;
    public ChatHistoryMdb(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("AI-Mongo-Db");
        _collection= database.GetCollection<ChatSessionModel>("ChatHistory");
    }
    public async Task<ChatSessionModel> CreateSession(int userId)
    {
        string sessionId =  ObjectId.GenerateNewId().ToString();;
        var model = new ChatSessionModel()
        {
            Id = sessionId,
            UserId = userId
        };
        await _collection.InsertOneAsync(model);
        return model;
    }

    public async Task AddHistory(AddChatHistoryReq req)
    {
        await _collection.UpdateOneAsync(
            session => session.Id == req.SessionId
            , Builders<ChatSessionModel>.Update.Push(model => model.History, new ChatHistoryModel()
            {
                Id =  ObjectId.GenerateNewId().ToString(),
                Content = req.Content,
                AuthorName = req.AuthorName,
                Role = req.Role,
            }));
    }

    public async Task<List<ChatSessionModel>> GetSessions(int userId,int limit=20)
    {
        var filter = Builders<ChatSessionModel>.Filter.Eq(s => s.UserId, userId);
        var sort = Builders<ChatSessionModel>.Sort.Descending(s => s.CreateDate); // ən yenilər önə
        var options = new FindOptions<ChatSessionModel>
        {
            Sort = sort,
            Limit = limit
        };

        using var cursor = await _collection.FindAsync(filter, options);
        return await cursor.ToListAsync();
    }
    
}