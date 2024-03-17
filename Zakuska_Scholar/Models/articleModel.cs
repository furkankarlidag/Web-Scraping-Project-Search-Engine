
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Zakuska_Scholar.Models
{
    public class articleModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        public int articleID { get; set; }

        public string articleName { get; set; }

        public string nameOfWriters { get; set; }

        public string typeOfArticle { get; set; }

        public string timeOfPublish { get; set; }

        public string nameOfPublisher { get; set; }

        public string searchKeys { get; set; }

        public string articleKeys { get; set; }

        public string summary { get; set; }

        public string summaryabs { get; set; }

        public string references { get; set; }

        public int numOfQuote { get; set; }

        public string url { get; set; }
    }
}
