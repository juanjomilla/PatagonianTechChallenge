using System.Net.Http;

namespace PatagonianChallengeAPI.Dao.Http
{
    public class HttpClientFactory : IHttpClientFactory
    {
        public HttpClient Create()
        {
            return new HttpClient();
        }
    }
}
