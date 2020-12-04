using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using PatagonianChallengeAPI.Dao.Http;
using PatagonianChallengeAPI.Models.Dao.DatabaseModels;
using PatagonianChallengeAPI.Models.Dao.SpotifyAPIModels;

namespace PatagonianChallengeAPI.Dao
{
    public class SongsDao : ISongsDao
    {
        private readonly ISessionFactory _sessionFactory;
        private readonly ISongHttpClient _songHttpClient;

        public SongsDao(ISessionFactory sessionFactory, ISongHttpClient songHttpClient)
        {
            _sessionFactory = sessionFactory;
            _songHttpClient = songHttpClient;
        }

        public async Task<IEnumerable<SongModel>> GetSongsByArtistNameAsync(string artistName, int limit, int offset)
        {
            using var session = _sessionFactory.OpenSession();
            var query = session.Query<SongModel>()
                .Where(x => x.Artist.Name.Contains(artistName))
                .Skip(offset)
                .Take(limit);

            return await query.ToListAsync();
        }

        public async Task<int> GetSongsCountAsync(string artistName)
        {
            using var session = _sessionFactory.OpenSession();
            var query = session.Query<SongModel>()
                .Where(x => x.Artist.Name.Contains(artistName));

            return (await query.ToListAsync()).Count;
        }

        public async Task<SongInfoModel> GetSongInfoByIdAsync(string songId)
        {
            try
            {
                return await _songHttpClient.GetSongInfoAsync(songId);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public async Task<SongModel> GetSongByIdAsync(string songId)
        {
            using var session = _sessionFactory.OpenSession();
            var query = session.Query<SongModel>()
                .Where(x => x.SongId == songId);

            return await query.SingleOrDefaultAsync();
        }
    }
}
