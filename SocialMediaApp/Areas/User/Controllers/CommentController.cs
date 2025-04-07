using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMediaApp.Data;
using SocialMediaApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMediaApp.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Add(int postId, string content)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || string.IsNullOrWhiteSpace(content))
                return BadRequest();

            var comment = new PostComment
            {
                PostId = postId,
                UserId = user.Id,
                Content = content.Trim(),
                CommentedAt = DateTime.Now
            };

            _context.PostComments.Add(comment);
            await _context.SaveChangesAsync();

          
            var post = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == comment.PostId);

            if (post != null && post.UserId != user.Id)
            {
                var notification = new Notification
                {
                    UserId = post.UserId,
                    Message = $"{user.Name ?? user.UserName} commented on your post.",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
            }
            

            return Json(new
            {
                success = true,
                user = user.Name,
                content = comment.Content,
                time = comment.CommentedAt.ToString("dd MMM yyyy, hh:mm tt"),
                profilePic = string.IsNullOrEmpty(user.ProfilePictureUrl) ? "/images/profile/default.jpg" : user.ProfilePictureUrl
            });
        }

    }
}
