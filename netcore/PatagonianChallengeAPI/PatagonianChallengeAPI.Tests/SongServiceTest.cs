using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using PatagonianChallengeAPI.Dao;
using PatagonianChallengeAPI.Dao.Http;
using PatagonianChallengeAPI.Models.Dao.DatabaseModels;
using PatagonianChallengeAPI.Models.Dao.SpotifyAPIModels;
using PatagonianChallengeAPI.Services;

namespace PatagonianChallengeAPI.Tests
{
    [TestClass]
    public class SongServiceTest
    {
        private static ISongsDao _songsDaoMock;

        [TestInitialize]
        public void Initialize()
        {
            _songsDaoMock = Substitute.For<ISongsDao>();
        }

        [TestMethod]
        public async Task GetSongsAsync_Should_Return_Collection_Of_Songs()
        {
            // Arrange
            var songListResultMock = new List<SongModel> 
            { 
                new SongModel
                {
                    SongId = "songIdMock",
                    SongName = "songName"
                }
            };

            _songsDaoMock
                .GetSongsByArtistNameAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
                .Returns(songListResultMock);

            var songService = new SongService(_songsDaoMock);

            // Act
            var songResult = await songService.GetSongsAsync("artistName", 20, 2);

            // Arrange
            Assert.IsNotNull(songResult);
            Assert.IsInstanceOfType(songResult, typeof(IEnumerable<SongModel>));
        }

        [TestMethod]
        public async Task GetSongsCountAsync_Should_Return_Amount_Of_Songs()
        {
            // Arrange
            var songsCountMock = 689;

            _songsDaoMock
                .GetSongsCountAsync(Arg.Any<string>())
                .Returns(songsCountMock);

            var songService = new SongService(_songsDaoMock);

            // Act
            var songCountResult = await songService.GetSongsCountAsync("artistName");

            // Arrange
            Assert.IsNotNull(songCountResult);
            Assert.IsTrue(songCountResult >= 0);
            Assert.IsInstanceOfType(songCountResult, typeof(int));
        }

        [TestMethod]
        public async Task GetSongInfoAsync_Should_Return_Song_Info()
        {
            // Arrange
            var songResultMock = new SongModel
            {
                SongId = "songIdMock",
                SongName = "songName"
            };

            var songInfoModel = new SongInfoModel
            {
                Id = "songIdMock",
                DiscNumber = 2,
                Name = "songName"
            };

            _songsDaoMock
                .GetSongByIdAsync(Arg.Any<string>())
                .Returns(songResultMock);

            _songsDaoMock
                .GetSongInfoByIdAsync(Arg.Any<string>())
                .Returns(songInfoModel);

            var songService = new SongService(_songsDaoMock);

            // Act
            var songInfoResult = await songService.GetSongInfoAsync("songId");

            // Arrange
            Assert.IsNotNull(songInfoResult);
            Assert.IsInstanceOfType(songInfoResult, typeof(SongInfoModel));
            Assert.AreEqual(songInfoModel.Id, songInfoResult.Id);
            Assert.AreEqual(songInfoModel.Name, songInfoResult.Name);
            Assert.AreEqual(songInfoModel.DiscNumber, songInfoResult.DiscNumber);
        }

        [TestMethod]
        public async Task GetSongInfoAsync_Should_Return_Null_If_Song_Not_Exists()
        {
            // Arrange
            _songsDaoMock
                .GetSongByIdAsync(Arg.Any<string>())
                .Returns((SongModel) null);

            var songService = new SongService(_songsDaoMock);

            // Act
            var songInfoResult = await songService.GetSongInfoAsync("songId");

            // Arrange
            Assert.IsNull(songInfoResult);
        }
    }
}
