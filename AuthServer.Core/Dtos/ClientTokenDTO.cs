namespace AuthServer.Core.Dtos;

public class ClientTokenDTO
{
    public string AccessToken { get; set; }
    public DateTime AccessTokenExpiration { get; set; }
}