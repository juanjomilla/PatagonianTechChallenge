using System.Collections.Generic;
using System.Threading.Tasks;
using PatagonianChallengeAPI.Dao;
using PatagonianChallengeAPI.Models.Dao.DatabaseModels;
using PatagonianChallengeAPI.Models.Dao.SpotifyAPIModels;

namespace PatagonianChallengeAPI.Services
{
    public class SongService : ISongService
    {
        private readonly ISongsDao _songsDao;

        public SongService(ISongsDao songsDao)
        {
            _songsDao = songsDao;
        }

        public async Task<IEnumerable<SongModel>> GetSongsAsync(string artistName, int offset, int limit)
        {
            return await _songsDao.GetSongsByArtistNameAsync(artistName, limit, offset);
        }

        public async Task<int> GetSongsCountAsync(string artistName)
        {
            return await _songsDao.GetSongsCountAsync(artistName);
        }

        public async Task<SongInfoModel> GetSongInfoAsync(string songId)
        {
            var song = await _songsDao.GetSongByIdAsync(songId);

            if (song == null)
            {
                return null;
            }

            return await _songsDao.GetSongInfoByIdAsync(songId);
        }
    }
}
