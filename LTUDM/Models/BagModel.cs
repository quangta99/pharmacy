using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTUDM.Models
{
    public class BagModel
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement]
        public string UserID { get; set; }
        public List<CartModel> ListCart { get; set; }
    }

   
}