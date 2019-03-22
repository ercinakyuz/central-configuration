using System.Runtime.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace CentralConfiguration.Cms.Data.Entity
{
    [DataContract]
    public class Configuration : BaseMongoEntity
    {
        [DataMember]
        [BsonIgnoreIfDefault]
        public string Key { get; set; }
        [DataMember]
        [BsonIgnoreIfDefault]
        public string Type { get; set; }
        [DataMember]
        [BsonIgnoreIfDefault]
        public string Value { get; set; }
        [DataMember]
        [BsonIgnoreIfDefault]
        public bool IsActive { get; set; }
        [DataMember]
        [BsonIgnoreIfDefault]
        public string ApplicationName { get; set; }
    }
}
