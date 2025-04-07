namespace SocialMediaApp.Models.ViewModel
{
    public class NotificationViewModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
