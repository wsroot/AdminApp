﻿namespace AdminApp.File.Models
{
    public class JwtSettings
    {
        public int Expires { get; set; }
        public string Issuer { get; set; }
        public string Key { get; set; }
    }
}