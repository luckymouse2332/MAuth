﻿namespace MAuth.Web.Models.DTOs
{
    public class AuthTokenDto
    {
        public string AccessToken { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;
    }
}
