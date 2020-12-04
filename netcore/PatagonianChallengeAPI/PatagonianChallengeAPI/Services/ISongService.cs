using System.Collections.Generic;
using System.Threading.Tasks;
using PatagonianChallengeAPI.Models.Dao.DatabaseModels;
using PatagonianChallengeAPI.Models.Dao.SpotifyAPIModels;

namespace PatagonianChallengeAPI.Services
{
    public interface ISongService
    {
        Task<IEnumerable<SongModel>> GetSongs(string artistName, int offset, int limit);

        Task<int> GetSongsCount(string artistName);

        Task<SongInfoModel> GetSongInfo(string songId);
    }
}
