using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMediaApp.Data;
using SocialMediaApp.Models;

namespace SocialMediaApp.Areas.Home.Controllers
{
    [Area("Home")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currentUserId = _userManager.GetUserId(User);

            var reportedPostIds = await _context.ReportedPosts
                .Where(r => r.UserId == currentUserId)
                .Select(r => r.PostId)
                .ToListAsync();

            var posts = await _context.Posts
                 .Where(p => !_context.ReportedPosts
            .Any(rp => rp.UserId == currentUserId && rp.PostId == p.Id))
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .OrderByDescending(p => p.PostedAt)
                .ToListAsync();

            return View(posts);
        }

    }
}
