﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTUDM.Models
{
    public class UserModel
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement]
        public string Email { get; set; }
        [BsonElement]
        public string Img { get; set; }

        [BsonElement]
        public string Name{ get; set; }
        [BsonElement]
        public string Phone { get; set; }
        [BsonElement]
        public string Direct { get; set; }
        [BsonElement]
        public string Password { get; set; }
        [BsonElement]
        public bool IsEmailVerfied { get; set; }
        [BsonElement]
        public System.Guid ActivationCode { get; set; }
    }

}