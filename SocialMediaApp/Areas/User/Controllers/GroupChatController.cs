using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SocialMediaApp.Data;
using SocialMediaApp.Models;
using SocialMediaApp.Models.ViewModel;
using Microsoft.EntityFrameworkCore;


namespace SocialMediaApp.Areas.User.Controllers
{
    [Area("User")]
    public class GroupChatController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GroupChatController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new GroupChatCreateViewModel
            {
                AllUsers = await GetFriendsAsync()
            };
            return View(model);
        }

        private async Task<List<ApplicationUser>> GetFriendsAsync()
        {
            var currentUserId = _userManager.GetUserId(User);

            var friends = await _context.FriendRequests
                .Where(fr => (fr.SenderId == currentUserId || fr.ReceiverId == currentUserId)
                    && fr.Status == FriendRequestStatus.Accepted)
                .Select(fr => fr.SenderId == currentUserId ? fr.Receiver : fr.Sender)
                .ToListAsync();

            return friends;
        }




        [HttpPost]
        public async Task<IActionResult> Create(GroupChatCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // 👇 Helps you debug what failed
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Key: {key}, Error: {error.ErrorMessage}");
                    }
                }

                model.AllUsers = await GetFriendsAsync(); // helper method
                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);

            var group = new GroupChat
            {
                GroupName = model.GroupName,
                CreatedByUserId = currentUser.Id
            };

            _context.GroupChats.Add(group);
            await _context.SaveChangesAsync();

            var usersToAdd = model.SelectedUserIds.Distinct().ToList();
            usersToAdd.Add(currentUser.Id); // Include creator

            foreach (var userId in usersToAdd)
            {
                _context.GroupChatUsers.Add(new GroupChatUser
                {
                    GroupChatId = group.Id,
                    UserId = userId
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("ChatList", "Chat");
        }


        //helper method to get friends list
        private async Task<List<ApplicationUser>> GetFriendList(string currentUserId)
        {
            return await _context.FriendRequests
                .Where(fr => (fr.SenderId == currentUserId || fr.ReceiverId == currentUserId) &&
                             fr.Status == FriendRequestStatus.Accepted)
                .Select(fr => fr.SenderId == currentUserId ? fr.Receiver : fr.Sender)
                .Distinct()
                .ToListAsync();
        }

        public async Task<IActionResult> MyGroupChats()
        {
            var userId = _userManager.GetUserId(User);

            var groups = await _context.GroupChatUsers
                .Include(gcu => gcu.GroupChat)
                .Where(gcu => gcu.UserId == userId)
                .Select(gcu => new GroupChatListViewModel
                {
                    GroupId = gcu.GroupChat.Id,
                    GroupName = gcu.GroupChat.GroupName
                })
                .ToListAsync();

            return View(groups);
        }


        public async Task<IActionResult> GroupChatRoom(int id)
        {
            var group = await _context.GroupChats.FindAsync(id);
            if (group == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var isMember = await _context.GroupChatUsers
                .AnyAsync(gcu => gcu.GroupChatId == id && gcu.UserId == userId);

            if (!isMember) return Forbid(); // restrict access if not a member

            ViewBag.GroupId = id;
            ViewBag.GroupName = group.GroupName;
            return View();
        }

    }
}
