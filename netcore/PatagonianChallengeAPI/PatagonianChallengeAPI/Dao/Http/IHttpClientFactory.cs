using System.Net.Http;

namespace PatagonianChallengeAPI.Dao.Http
{
    public interface IHttpClientFactory
    {
        HttpClient Create();
    }
}
