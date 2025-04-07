using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaApp.Models
{
    public class GroupChat
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string GroupName { get; set; }

        public string CreatedByUserId { get; set; }

        [ForeignKey("CreatedByUserId")]
        public ApplicationUser CreatedByUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<GroupChatUser> Members { get; set; }

        public ICollection<GroupMessage> Messages { get; set; }
    }


}
