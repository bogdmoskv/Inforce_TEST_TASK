namespace Inforce_.NET_Task_Moskvichev_Bogdan.Models
{
    public class UrlManagement
    {
        public int Id { get; set; }
        public string ? Url { get; set; }=string.Empty;
        public string ? ShortUrl { get; set; } = string.Empty;
        public int OwnerId { get; set; } = 0;
        public DateTime ? CreatedTime { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time"));
    }
}
