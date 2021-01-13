using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTUDM.Areas.Admin.Models
{
    public class AdminModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement]
        public string AdminName { get; set; }

        [BsonElement]
        public string Name { get; set; }
        [BsonElement]
        public string Password { get; set; }

        [BsonElement]
        public string Img { get; set; }
        [BsonElement]
        public string Email { get; set; }
    }
}