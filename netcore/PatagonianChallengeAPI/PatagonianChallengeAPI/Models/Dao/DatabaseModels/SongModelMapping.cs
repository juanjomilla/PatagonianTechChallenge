using FluentNHibernate.Mapping;

namespace PatagonianChallengeAPI.Models.Dao.DatabaseModels
{
    public class SongModelMapping : ClassMap<SongModel>
    {
        public SongModelMapping()
        {
            Id(x => x.SongId, "id");
            Map(x => x.SongName, "name");
            References(x => x.Artist, "artist_id");
            Table("track");
        }
    }
}
