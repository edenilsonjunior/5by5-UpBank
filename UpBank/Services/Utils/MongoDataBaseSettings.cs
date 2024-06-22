namespace Services.Utils
{
    public class MongoDataBaseSettings : IMongoDataBaseSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string AddressCollectionName { get; set; }
    }
}
