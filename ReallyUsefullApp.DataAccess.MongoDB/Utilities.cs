using MongoDB.Bson;
using Newtonsoft.Json.Linq;

namespace ReallyUsefullApp.DataAccess.MongoDB
{
    public static class Utilities
    {
        public static dynamic ToDynamic(this BsonDocument doc)
        {
            var json = doc.ToJson();
            dynamic obj = JToken.Parse(json);
            return obj;
        }
    }
}
