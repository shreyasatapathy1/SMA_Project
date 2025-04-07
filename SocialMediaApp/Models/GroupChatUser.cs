using System.ComponentModel.DataAnnotations.Schema;

namespace SocialMediaApp.Models
{
    public class GroupChatUser
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public int GroupChatId { get; set; }
        [ForeignKey("GroupChatId")]
        public GroupChat GroupChat { get; set; }

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }

}
