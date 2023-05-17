using MongoDB.Bson;
using MongoDB.Driver;

namespace spending_money_control_api
{
    public class constants
    {
        public static string host = "spendingmoneycontrolapi20230428130151.azurewebsites.net";
        public static string botId = "6033500949:AAGLxuMrDbgEXgOUE6Y5L_itJQUc8jUvsj4";
        public static string crypto_api_key = $"f75dea3fd33f7a114d35e70d5a21633605dbbf664c959450306227a460f7";
        public static string crypto_api_url = $"https://api.cryptorank.io/v0/coins/bitcoin";
        public static MongoClient mongoClient;
        public static IMongoDatabase database;
        public static IMongoCollection<BsonDocument> collection;
        public static IMongoCollection<BsonDocument> collection2;
        public static string urlUserInfo = $"https://api.monobank.ua/personal/client-info";
    }
}
