using LMS.Domain.Entities;
using LMS.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LMS.Infrastructure.Persistence;

/// <summary>Provides typed access to the MongoDB collections used by the LMS.</summary>
public class MongoContext
{
    private readonly IMongoDatabase _database;

    public MongoContext(IOptions<MongoSettings> options)
    {
        MongoMappingConfig.Register();
        var client = new MongoClient(options.Value.ConnectionString);
        _database = client.GetDatabase(options.Value.Database);
    }

    // Collection names match the Mongoose pluralised model names so existing data is reused.
    public IMongoCollection<User> Users => _database.GetCollection<User>("users");
    public IMongoCollection<Course> Courses => _database.GetCollection<Course>("courses");
    public IMongoCollection<Order> Orders => _database.GetCollection<Order>("orders");
    public IMongoCollection<Notification> Notifications => _database.GetCollection<Notification>("notifications");
    public IMongoCollection<Layout> Layouts => _database.GetCollection<Layout>("layouts");
}
