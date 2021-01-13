using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTUDM.Models
{
    public class BannerModels
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement]
        public string BannerName { get; set; }

        [BsonElement]
        public string Img { get; set; }
    }
}