using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NHibernate;
using NHibernate.Linq;
using PatagonianChallengeAPI.Models.Dao.DatabaseModels;
using PatagonianChallengeAPI.Models.Dao.SpotifyAPIModels;

namespace PatagonianChallengeAPI.Dao
{
    public class SongsDao : ISongsDao
    {
        private const string authUrl = "https://accounts.spotify.com/api/token";
        private const string trackInfoUrl = "https://api.spotify.com/v1/tracks/{0}";

        private readonly ISessionFactory _sessionFactory;
        private readonly IConfiguration _configuration;

        public SongsDao(ISessionFactory sessionFactory, IConfiguration configuration)
        {
            _configuration = configuration;
            _sessionFactory = sessionFactory;
        }

        public async Task<IEnumerable<SongModel>> GetSongsAsync(string artistName, int limit, int offset)
        {
            using var session = _sessionFactory.OpenSession();
            var query = session.Query<SongModel>()
                .Where(x => x.Artist.Name.Contains(artistName))
                .Skip(offset)
                .Take(limit);

            return await query.ToListAsync();
        }

        public async Task<int> GetSongsCountAsync(string artistName)
        {
            using var session = _sessionFactory.OpenSession();
            var query = session.Query<SongModel>()
                .Where(x => x.Artist.Name.Contains(artistName));

            return (await query.ToListAsync()).Count;
        }

        public async Task<SongInfoModel> GetSongInfoByIdAsync(string songId)
        {
            try
            {
                var authorizationToken = await GetAuthorizationTokenAsync();
                var url = string.Format(trackInfoUrl, songId);

                using var httpClient = new HttpClient();
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

        public async Task<SongModel> GetSongByIdAsync(string songId)
        {
            using var session = _sessionFactory.OpenSession();
            var query = session.Query<SongModel>()
                .Where(x => x.SongId == songId);

            return await query.SingleOrDefaultAsync();
        }

        private async Task<string> GetAuthorizationTokenAsync()
        {
            var clientId = _configuration.GetSection("spotifyClientId").Value;
            var secretKey = _configuration.GetSection("spotifySecretKey").Value;
            var base64EncodedAuth = System.Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{secretKey}"));

            using var httpClient = new HttpClient();
            using var httpContent = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuth);
            
            var response = await httpClient.PostAsync(authUrl, httpContent);
            response.EnsureSuccessStatusCode();

            var contentResponse = await response.Content.ReadAsStringAsync();
            var responseJDocument = JsonDocument.Parse(contentResponse);

            return responseJDocument.RootElement.GetProperty("access_token").GetString();
        }
    }
}
