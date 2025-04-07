using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialMediaApp.Data;
using SocialMediaApp.Models;
using SocialMediaApp.Models.ViewModel;

namespace SocialMediaApp.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            var unreadCount = notifications.Count(n => !n.IsRead);
            ViewBag.UnreadCount = unreadCount;

            return View(notifications);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
                return NotFound();

            _context.Notifications.Remove(notification); 
            await _context.SaveChangesAsync();

            return Ok();
        }


    }
}
