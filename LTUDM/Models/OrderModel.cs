using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTUDM.Models
{
    public class OrderModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement]
        public string OrderID { get; set; }

        [BsonElement]
        public string OrderDate { get; set; }

        [BsonElement]
        public string UserID { get; set; }

        [BsonElement]
        public string Phone { get; set; }

        [BsonElement]
        public string OrderStatus { get; set; }
        [BsonElement]
        public string Address { get; set; }
        [BsonElement]
        public string Name { get; set; }
        [BsonElement]
        public string Email { get; set; }
        [BsonElement]
        public string Notes { get; set; }

        [BsonElement]
        public Decimal TotalPrice { get; set; }

    }
}