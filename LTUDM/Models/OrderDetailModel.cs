using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTUDM.Models
{
    public class OrderDetailModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement]
        public string OrderID { get; set; }

        [BsonElement]
        public string ProductID { get; set; }

        [BsonElement]
        public int Quantity { get; set; }
        [BsonElement]
        public Decimal SalePrice { get; set; }
    }
}