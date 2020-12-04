using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PatagonianChallengeAPI.Models.Dao.SpotifyAPIModels
{
    public class SongInfoModel
    {
        [JsonPropertyName("artists")]
        public IEnumerable<ArtistModel> Artists { get; set; }

        [JsonPropertyName("disc_number")]
        public int DiscNumber { get; set; }

        [JsonPropertyName("duration_ms")]
        public int Duration { get; set; }

        [JsonPropertyName("explicit")]
        public bool Explicit { get; set; }

        [JsonPropertyName("external_urls")]
        public ExternalUrlsModel ExternalUrls { get; set; }

        [JsonPropertyName("href")]
        public string HRef { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("is_local")]
        public bool IsLocal { get; set; }

        [JsonPropertyName("is_playable")]
        public bool IsPlayable { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("preview_url")]
        public string PreviewUrl { get; set; }

        [JsonPropertyName("track_number")]
        public int TrackNumber { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("uri")]
        public string SpotifyUri { get; set; }
    }
}
