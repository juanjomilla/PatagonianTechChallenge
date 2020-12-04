using System.Collections.Generic;

namespace PatagonianChallengeAPI.Models.Dao.DatabaseModels
{
    public class ArtistModel
    {
        public virtual string Id { get; set; }

        public virtual string Name { get; set; }

        public virtual IEnumerable<SongModel> Songs { get; set; }
    }
}
