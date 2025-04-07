using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaApp.Models.ViewModel
{
    public class GroupChatCreateViewModel
    {
        [Required]
        [StringLength(100)]
        public string GroupName { get; set; }

        [Required]
        public List<string> SelectedUserIds { get; set; }

        public List<ApplicationUser> AllUsers { get; set; }
    }

}
