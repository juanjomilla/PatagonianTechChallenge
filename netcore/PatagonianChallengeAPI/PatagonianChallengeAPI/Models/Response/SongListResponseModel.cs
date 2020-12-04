using System.Collections.Generic;
using PatagonianChallengeAPI.Models.Dao.DatabaseModels;

namespace PatagonianChallengeAPI.Models.Response
{
    public class SongListResponseModel
    {
        public IEnumerable<SongResponseModel> Songs { get; set; }

        public string Next { get; set; }

        public SongListResponseModel(IEnumerable<SongModel> songsModel)
        {
            var songs = new List<SongResponseModel>();

            foreach (var songModel in songsModel)
            {
                songs.Add(new SongResponseModel { SongId = songModel.SongId, SongName = songModel.SongName });
            }

            Songs = songs;
        }
    }
}
