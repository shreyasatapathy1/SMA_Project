using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using SocialMediaApp.Data;
using SocialMediaApp.Models;
using SocialMediaApp.Models.ViewModel;
using Microsoft.AspNetCore.Identity;

namespace SocialMediaApp.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public PostController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostViewModel model)
        {
            Console.WriteLine("ModelState is valid: " + ModelState.IsValid);
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine("Model Error: " + error.ErrorMessage);
            }

            Console.WriteLine("Post Create POST action entered");
            Console.WriteLine("MediaFile: " + model.MediaFile?.FileName);
            Console.WriteLine("MediaFile Length: " + model.MediaFile?.Length);
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                Console.WriteLine("User is null");
                return Unauthorized();
            }
            Console.WriteLine("MediaFile: " + model.MediaFile?.FileName);
            Console.WriteLine("MediaFile Length: " + model.MediaFile?.Length);

            if (string.IsNullOrWhiteSpace(model.TextContent) && (model.MediaFile == null || model.MediaFile.Length == 0))
            {
                ModelState.AddModelError("", "Please write something or upload an image/video.");
                Console.WriteLine("Post rejected due to empty content");
                return View(model);
            }

            string? mediaUrl = null;
            string mediaType = "none";

            if (model.MediaFile != null && model.MediaFile.Length > 0)
            {
                var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsDir))
                    Directory.CreateDirectory(uploadsDir);


                var fileName = Guid.NewGuid() + Path.GetExtension(model.MediaFile.FileName);
                var filePath = Path.Combine(uploadsDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.MediaFile.CopyToAsync(stream);
                    Console.WriteLine($"Saved file to: {filePath}");

                }

                mediaUrl = "/uploads/" + fileName;

                //var ext = Path.GetExtension(fileName).ToLower();
                //if (ext == ".mp4" || ext == ".webm")
                //    mediaType = "video";
                //else if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".gif")
                //    mediaType = "image";

                var ext = Path.GetExtension(fileName).ToLowerInvariant(); // use .ToLowerInvariant()

                if (ext == ".mp4" || ext == ".webm")
                    mediaType = "video";
                else if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".gif" || ext == ".jfif")
                    mediaType = "image";
                else
                    mediaType = "none"; 

            }

            var post = new Post
            {
                TextContent = model.TextContent,
                MediaUrl = mediaUrl,
                MediaType = mediaType,
                UserId = user.Id,
                PostedAt = DateTime.Now
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            Console.WriteLine("Post saved successfully");
            TempData["Success"] = "Post created successfully!";
            return RedirectToAction("MyPosts");
        }



        [HttpGet]
        public async Task<IActionResult> MyPosts()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                Console.WriteLine("User not found while loading MyPosts");
                return Unauthorized();
            }

            var posts = _context.Posts
                        .Where(p => p.UserId == user.Id)
                        .OrderByDescending(p => p.PostedAt)
                        .ToList();

            Console.WriteLine($"Found {posts.Count} posts for user {user.Id}");
            return View(posts);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePost(int id, string newText)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null || post.UserId != _userManager.GetUserId(User))
                return NotFound();

            post.TextContent = newText;
            await _context.SaveChangesAsync();

            return Ok(new { success = true, updatedText = post.TextContent });
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null || post.UserId != _userManager.GetUserId(User))
                return NotFound();

            if (!string.IsNullOrEmpty(post.MediaUrl))
            {
                var path = Path.Combine(_env.WebRootPath, post.MediaUrl.TrimStart('/'));
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }
            // Unlink shared post references in messages
            var sharedMessages = _context.Messages
                .Where(m => m.SharedPostId == post.Id)
                .ToList();

            foreach (var msg in sharedMessages)
            {
                msg.SharedPostId = null;
            }
            _context.SaveChanges();

            // Remove comments
            var comments = _context.PostComments.Where(c => c.PostId == post.Id);
            _context.PostComments.RemoveRange(comments);

            // Remove likes
            var likes = _context.PostLikes.Where(l => l.PostId == post.Id);
            _context.PostLikes.RemoveRange(likes);

            // Finally delete the post
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            //_context.Posts.Remove(post);
            //await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }


        [HttpGet]
        public IActionResult GetShareCount(int postId)
        {
            var count = _context.Messages.Count(m => m.SharedPostId == postId);
            return Json(new { count });
        }




    }
}
