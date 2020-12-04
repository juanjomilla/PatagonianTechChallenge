using System.Collections.Generic;
using System.Threading.Tasks;
using PatagonianChallengeAPI.Models.Dao.DatabaseModels;
using PatagonianChallengeAPI.Models.Dao.SpotifyAPIModels;

namespace PatagonianChallengeAPI.Services
{
    public interface ISongService
    {
        Task<IEnumerable<SongModel>> GetSongsAsync(string artistName, int offset, int limit);

        Task<int> GetSongsCountAsync(string artistName);

        Task<SongInfoModel> GetSongInfoAsync(string songId);
    }
}
