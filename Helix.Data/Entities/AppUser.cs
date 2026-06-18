using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Helix.Data.Entities
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = default!;
        public string MiddleName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string NationalId { get; set; } = default!;
        public string Address { get; set; } = default!;
        public bool NotificationsEnabled { get; set; }
        public string ProfilePictureUrl { get; set; } = default!;
    }
}
