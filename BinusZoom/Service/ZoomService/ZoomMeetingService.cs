using System.Text;
using BinusZoom.Service.ZoomService.DTO;
using Newtonsoft.Json;

namespace BinusZoom.Service.ZoomService;

public class ZoomMeetingService
{
    public ZoomAccountList ZoomAccountList { get; set; }
    
    public ZoomMeetingService(ZoomAccountList zoomAccountList)
    {
        ZoomAccountList = zoomAccountList;
    }
    
    public async Task GetParticipantList(string meetingId)
    {
        foreach (var account in ZoomAccountList.accounts)
        {
            string accessToken = await GetAccessToken(ZoomAccountList.AccessTokenAuthUrl, account.ClientId, account.ClientSecret, account.AccountId);
            
            ZoomAccountList.PastMeetingUrl = ZoomAccountList.PastMeetingUrl.Replace("{meetingId}", meetingId);
        }
    }
    
    private async Task<string> GetAccessToken(string url, string clientId, string clientSecret, string accountId)
    {
        try
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
            Dictionary<string, string> responseContent = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);
            return responseContent.TryGetValue("access_token", out string accessToken) ? accessToken : throw new Exception("Access token not found");
        } catch (Exception e)
        {
            throw e;
        }
    }
}