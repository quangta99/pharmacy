using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTUDM.Models
{
    public class CategoryModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement]
        public string CategoryID { get; set; }

        [BsonElement]
        public string CategoryName { get; set; }


    }
}