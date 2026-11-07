using System;
using System.Collections.Generic;
using System.Text;

namespace MyApp.Application.Services
{
    public class JwtSettings
    {
        public string Secret { get; set; } = string.Empty;
        public int ExpirationMinute { get; set; }
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}
