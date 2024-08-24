namespace AuthServer.Core.Configuration;

public class Client
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }

    //www.myapi1.com www.myapi2.com
    public List<string> Audieneces { get; set; }
}