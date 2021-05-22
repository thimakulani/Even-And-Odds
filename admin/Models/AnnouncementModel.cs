using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;

namespace admin.Models
{
    public class AnnouncementModel
    {
        public string Message { get; set; }
        public string Id { get; set; }
        [ServerTimestamp]
        public Timestamp TimeStamp { get; set; }
    }
}