using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialMediaApp.Models
{
    public class GroupMessage
    {
        public int Id { get; set; }

        [Required]
        public string SenderId { get; set; }
        [ForeignKey("SenderId")]
        public ApplicationUser Sender { get; set; }

        public int GroupChatId { get; set; }
        [ForeignKey("GroupChatId")]
        public GroupChat GroupChat { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }

}
