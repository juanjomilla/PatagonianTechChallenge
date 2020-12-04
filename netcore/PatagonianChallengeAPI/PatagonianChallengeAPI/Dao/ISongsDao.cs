using System.Collections.Generic;
using System.Threading.Tasks;
using PatagonianChallengeAPI.Models.Dao.DatabaseModels;
using PatagonianChallengeAPI.Models.Dao.SpotifyAPIModels;

namespace PatagonianChallengeAPI.Dao
{
    public interface ISongsDao
    {
        Task<IEnumerable<SongModel>> GetSongsByArtistNameAsync(string artistName, int limit, int offset);

        Task<int> GetSongsCountAsync(string artistName);

        Task<SongModel> GetSongByIdAsync(string songId);

        Task<SongInfoModel> GetSongInfoByIdAsync(string songId);
    }
}
