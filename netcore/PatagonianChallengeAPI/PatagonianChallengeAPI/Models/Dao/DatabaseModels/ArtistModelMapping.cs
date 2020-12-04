using FluentNHibernate.Mapping;

namespace PatagonianChallengeAPI.Models.Dao.DatabaseModels
{
    public class ArtistModelMapping : ClassMap<ArtistModel>
    {
        public ArtistModelMapping()
        {
            Id(x => x.Id, "id");
            Map(x => x.Name, "name");
            HasMany(x => x.Songs)
                .KeyColumn("artist_id");
            Table("artist");
        }
    }
}
