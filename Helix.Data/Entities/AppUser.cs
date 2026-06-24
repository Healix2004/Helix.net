using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Helix.Data.Entities
{
    public class AppUser : IdentityUser
    {
        public bool NotificationsEnabled { get; set; } = true;
    }
}
