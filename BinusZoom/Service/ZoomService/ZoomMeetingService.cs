using System.Net;
using System.Text;
using BinusZoom.Service.ZoomService.DTO;
using Newtonsoft.Json;

namespace BinusZoom.Service.ZoomService;

public class ZoomMeetingService
{
    public ZoomMeetingService(ZoomAccountList zoomAccountList)
    {
        ZoomAccountList = zoomAccountList;
    }

    public ZoomAccountList ZoomAccountList { get; set; }

    public async Task GetParticipantList(string meetingId)
    {
        var meetingParticipantUrl = ZoomAccountList.PastMeetingParticipantUrlPattern.Replace("{meetingId}", meetingId);

        foreach (var account in ZoomAccountList.accounts)
        {
            var accessToken = await GetAccessToken(ZoomAccountList.AccessTokenAuthUrl, account.ClientId,
                account.ClientSecret, account.AccountId);
            account.AccessToken = accessToken;
        }
    }


    private async Task<string> GetAccessToken(string url, string clientId, string clientSecret, string accountId)
    {
        byte[] clientIdAndSecret = Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}");
        string authHeader = Convert.ToBase64String(clientIdAndSecret);
            
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("Authorization", $"Basic {authHeader}");
        var collection = new List<KeyValuePair<string, string>>()
        {
            new("grant_type", "account_credentials"),
            new("account_id", accountId)
        };
            
        var content = new FormUrlEncodedContent(collection);
        request.Content = content;
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
            
        var responseString = await response.Content.ReadAsStringAsync();
        Dictionary<string, string>? responseContent = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);
        return responseContent != null && responseContent.TryGetValue("access_token", out string accessToken) ? accessToken : throw new Exception("Access token not found");
    }
}