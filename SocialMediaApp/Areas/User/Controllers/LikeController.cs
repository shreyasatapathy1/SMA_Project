using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMediaApp.Data;
using SocialMediaApp.Models;

namespace SocialMediaApp.Areas.User.Controllers
{
    [Authorize]
    [Area("User")]
    public class LikeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public LikeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Toggle(int postId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var like = _context.PostLikes.FirstOrDefault(pl => pl.PostId == postId && pl.UserId == currentUser.Id);
            bool liked;

            if (like != null)
            {
                _context.PostLikes.Remove(like);
                liked = false;
            }
            else
            {
                _context.PostLikes.Add(new PostLike
                {
                    PostId = postId,
                    UserId = currentUser.Id,
                    LikedAt = DateTime.Now
                });
                liked = true;

                
                var post = await _context.Posts
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == postId);

                if (post != null && post.UserId != currentUser.Id)
                {
                    var notification = new Notification
                    {
                        UserId = post.UserId,
                        Message = $"{currentUser.Name ?? currentUser.UserName} liked your post.",
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Notifications.Add(notification);
                }
            }

            await _context.SaveChangesAsync();

            var totalLikes = _context.PostLikes.Count(pl => pl.PostId == postId);

            return Json(new { liked, totalLikes });
        }

    }

}
