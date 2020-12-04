namespace PatagonianChallengeAPI.Models.Dao.DatabaseModels
{
    public class SongModel
    {
        public virtual string SongId { get; set; }

        public virtual string SongName { get; set; }

        public virtual ArtistModel Artist { get; set; }
    }
}
