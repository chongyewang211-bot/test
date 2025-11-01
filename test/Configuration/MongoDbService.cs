using MongoDB.Driver;
using test.Configuration;

namespace test.Configuration
{
    public class MongoDbService
    {
        private readonly IMongoDatabase _database;

        public MongoDbService(MongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);
        }

        public IMongoDatabase Database => _database;
    }
}
