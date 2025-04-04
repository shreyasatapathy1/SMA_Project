﻿using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace SocialMediaApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? ProfilePictureUrl { get; set; }
        public string? Name { get; set; }
        public string? Bio { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }

       
    }
}
