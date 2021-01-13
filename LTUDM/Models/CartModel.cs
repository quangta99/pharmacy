using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTUDM.Models
{
    public class CartModel
    {

        [BsonElement]
        public string UserID { get; set; }

        [BsonElement]
        public string ProductID { get; set; }

        [BsonElement]
        public int Quantity { get; set; }

    }
    
}