﻿using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;

namespace admin.Models
{
    public  class QueriesModel
    {
        [ServerTimestamp]
        public FieldValue TimeStamp { get; set; }
        public string Uid { get; set; }
    }
}