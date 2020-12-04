using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PatagonianChallengeAPI.Models.Dao.SpotifyAPIModels;
using PatagonianChallengeAPI.Models.Response;
using PatagonianChallengeAPI.Services;

namespace PatagonianChallengeAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SongsController : ControllerBase
    {
        private readonly ISongService _songService;

        public SongsController(ISongService songService)
        {
            _songService = songService;
        }

        [HttpGet]
        public async Task<ActionResult<SongListResponseModel>> GetSongs(
            [FromQuery]string artistName,
            [FromQuery]int limit = 20,
            [FromQuery]int offset = 0)
        {
            if (string.IsNullOrWhiteSpace(artistName) || artistName.Length < 3)
            {
                return BadRequest("The 'artistName' parameter is mandatory, should be not empty and should have at least 3 characters");
            }

            var songs = await _songService.GetSongsAsync(artistName, offset, limit);
            var songsCount = await _songService.GetSongsCountAsync(artistName);

            var queryParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("artistName", artistName),
                new KeyValuePair<string, string>("offset", (offset + limit).ToString()),
                new KeyValuePair<string, string>("limit", limit.ToString())
            };

            var songsResponse = new SongListResponseModel(songs)
            {
                Next = songsCount >= offset + limit + 1 ? BuildNextUrl(queryParams) : null
            };

            return Ok(songsResponse);
        }

        [HttpGet]
        [Route("{songId}")]
        public async Task<ActionResult<SongInfoModel>> GetSong([FromRoute]string songId)
        {
            var song = await _songService.GetSongInfoAsync(songId);
            if (song == null)
            {
                return NotFound(new { message = "Song not found" });
            }

            return Ok(song);
        }

        private string BuildNextUrl(IEnumerable<KeyValuePair<string, string>> queryParams)
        {
            var fullUrl = string.Format(
                "{0}://{1}{2}",
                HttpContext.Request.Scheme,
                HttpContext.Request.Host.ToUriComponent(),
                HttpContext.Request.Path.ToUriComponent());

            var sb = new StringBuilder(fullUrl + "?");
            foreach (var param in queryParams)
            {
                sb.Append($"{param.Key}={param.Value}&");
            }

            return sb
                .ToString()
                .TrimEnd('&', '?');
        }
    }
}
