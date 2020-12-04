using System.Threading.Tasks;
using PatagonianChallengeAPI.Models.Dao.SpotifyAPIModels;

namespace PatagonianChallengeAPI.Dao.Http
{
    public interface ISongHttpClient
    {
        Task<SongInfoModel> GetSongInfoAsync(string songId);
    }
}
