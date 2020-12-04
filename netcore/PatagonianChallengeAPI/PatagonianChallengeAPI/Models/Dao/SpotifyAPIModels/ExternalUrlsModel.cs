using System.Text.Json.Serialization;

namespace PatagonianChallengeAPI.Models.Dao.SpotifyAPIModels
{
    public class ExternalUrlsModel
    {
        [JsonPropertyName("spotify")]
        public string SpotifyUrl { get; set; }
    }
}
