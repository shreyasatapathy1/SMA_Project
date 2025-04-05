using System.ComponentModel.DataAnnotations;

namespace SocialMediaApp.Models
{
    public class PostComment
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Required]
        public int PostId { get; set; }
        public Post Post { get; set; }

        [Required]
        [MaxLength(300)]
        public string Content { get; set; }

        public DateTime CommentedAt { get; set; } = DateTime.Now;
    }

}
