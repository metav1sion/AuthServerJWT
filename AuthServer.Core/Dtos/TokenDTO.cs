﻿namespace AuthServer.Core.Dtos;

public class TokenDTO
{
    public string AccessToken { get; set; }
    public DateTime AccessTokenExpiration { get; set; }
    public string RefreshToken { get; set; }
    public string RefreshTokenExpiration { get; set; }
}