using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;

namespace admin.Models
{
    public class Replies
    {
        public string Message { get; set; }
        public string Id { get; set; }
        public string Uid { get; set; }
        [ServerTimestamp]
        public FieldValue TimeStamp { get; set; }
    }
}