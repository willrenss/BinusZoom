namespace BinusZoom.Service.ZoomService.DTO;

public class ZoomAccountList
{
    public List<ZoomCredential> accounts { get; set; } = new List<ZoomCredential>();
    public string AccessTokenAuthUrl { get; set; }
    public string PastMeetingUrl { get; set; }
}

public class ZoomCredential
{
    public string AccountId { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string AccessToken { get; set; }
}