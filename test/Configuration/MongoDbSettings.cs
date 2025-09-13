namespace test.Configuration
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string UsersCollectionName { get; set; } = "users";
        public string ProblemsCollectionName { get; set; } = "problems";
        public string CategoriesCollectionName { get; set; } = "categories";
    }
}
