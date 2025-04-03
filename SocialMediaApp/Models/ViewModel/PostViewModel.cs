using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaApp.Models.ViewModel
{
    public class PostViewModel
    {
        public string? TextContent { get; set; }
        public IFormFile? MediaFile { get; set; }
    }

}
