using System.ComponentModel.DataAnnotations.Schema;

namespace SocialMediaApp.Models
{
    public class ReportedPost
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public int PostId { get; set; }
        [ForeignKey("PostId")]
        public Post Post { get; set; }

        public DateTime ReportedAt { get; set; } = DateTime.UtcNow;
    }

}
