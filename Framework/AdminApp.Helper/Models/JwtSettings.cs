﻿namespace AdminApp.Helper.Models
{
    public class JwtSettings
    {
        public string Audience { get; set; }
        public int Expires { get; set; }
        public string Issuer { get; set; }
        public string Key { get; set; }
    }
}
