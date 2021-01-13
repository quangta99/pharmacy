using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace LTUDM.Models
{
    public class ProductModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement]
        public string ProductID { get; set; }

        [BsonElement]
        public string ProductName { get; set; }

        [BsonElement]
        public int discount { get; set; }
        [BsonElement]
        public decimal Price { get; set; }

        [BsonElement]
        public string Description { get; set; }
        [BsonElement]
        public int Quantity { get; set; }
        [BsonElement]
        public List<string> img { get; set; }
        [BsonElement]
        public string Producer { get; set; }

        [BsonElement]
        public bool status { get; set; }
        [BsonElement]
        public TypeM Type { get; set; }


    }
    public class TypeM
    {
        [BsonElement]
        public string CategoryID { get; set; }
        [BsonElement]
        public string CategoryName { get; set; }

    }
}