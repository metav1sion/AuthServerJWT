﻿namespace AuthServer.Core.Entities;

public class UserRefreshToken
{
    public string UserId { get; set; }
    public string Code { get; set; } //Refresh token code
    public DateTime Expiration { get; set; }
}