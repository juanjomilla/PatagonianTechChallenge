using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PatagonianChallengeAPI.Models.Dao.SpotifyAPIModels;

namespace PatagonianChallengeAPI.Dao.Http
{
    public class SongHttpClient : ISongHttpClient
    {
        private const string authUrl = "https://accounts.spotify.com/api/token";
        private const string trackInfoUrl = "https://api.spotify.com/v1/tracks/{0}";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public SongHttpClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<SongInfoModel> GetSongInfoAsync(string songId)
        {
            try
            {
                using var httpClient = _httpClientFactory.Create();
                var url = string.Format(trackInfoUrl, songId);

                var authorizationToken = await GetAuthorizationTokenAsync();

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorizationToken);

                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var contentResponse = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<SongInfoModel>(contentResponse);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        private async Task<string> GetAuthorizationTokenAsync()
        {
            var clientId = _configuration.GetSection("spotifyClientId").Value;
            var secretKey = _configuration.GetSection("spotifySecretKey").Value;
            var base64EncodedAuth = System.Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{secretKey}"));

            try
            {
                using var httpClient = _httpClientFactory.Create();
                using var httpContent = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuth);

                var response = await httpClient.PostAsync(authUrl, httpContent);
                response.EnsureSuccessStatusCode();

                var contentResponse = await response.Content.ReadAsStringAsync();
                var responseJDocument = JsonDocument.Parse(contentResponse);

                return responseJDocument.RootElement.GetProperty("access_token").GetString();
            }
            catch (System.Exception)
            {
                throw;
            }

        }
    }
}
