using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helix.Service.Settings
{
    public class JwtSettings
    {
        const string SectionName = "JwtSettings";
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public string Secret { get; set; } = default!;
        public int ExpirationInDay { get; set; }
    }
}
