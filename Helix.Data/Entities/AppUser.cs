using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Helix.Data.Entities
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } = default!;
        public string NationalId { get; set; } = default!;
        public bool NotificationsEnabled { get; set; } = true;
        public string? ProfilePictureUrl { get; set; }
        public string? Address { get; set; }
    }
}
