using System.Text.Json.Serialization;

namespace PatagonianChallengeAPI.Models.Dao.SpotifyAPIModels
{
    public class ArtistModel
    {
        [JsonPropertyName("external_urls")]
        public ExternalUrlsModel ExternalUrls { get; set; }

        [JsonPropertyName("href")]
        public string HRef { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("uri")]
        public string SpotifyUri { get; set; }
    }
}
