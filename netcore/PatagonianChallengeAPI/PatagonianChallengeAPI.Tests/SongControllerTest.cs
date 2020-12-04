using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using PatagonianChallengeAPI.Controllers;
using PatagonianChallengeAPI.Models.Dao.DatabaseModels;
using PatagonianChallengeAPI.Models.Dao.SpotifyAPIModels;
using PatagonianChallengeAPI.Models.Response;
using PatagonianChallengeAPI.Services;

namespace PatagonianChallengeAPI.Tests
{
    [TestClass]
    public class SongControllerTest
    {
        private static ISongService _songServiceMock;

        [TestInitialize]
        public void Initialize()
        {
            _songServiceMock = Substitute.For<ISongService>();
        }

        [TestMethod]
        public async Task GetSongs_Should_Return_BadRequest_If_ArtistName_Is_Null()
        {
            // Arrange
            var songsController = new SongsController(_songServiceMock);

            // Act
            var result = await songsController.GetSongs(null);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task GetSongs_Should_Return_BadRequest_If_ArtistName_Is_Empty()
        {
            // Arrange
            var songsController = new SongsController(_songServiceMock);

            // Act
            var result = await songsController.GetSongs("");

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task GetSongs_Should_Return_BadRequest_If_ArtistName_Is_WhiteSpace()
        {
            // Arrange
            var songsController = new SongsController(_songServiceMock);

            // Act
            var result = await songsController.GetSongs("     ");

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task GetSongs_Should_Return_BadRequest_If_ArtistName_Length_Is_Less_Than_3()
        {
            // Arrange
            var songsController = new SongsController(_songServiceMock);

            // Act
            var result = await songsController.GetSongs("qu");

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task GetSongs_Should_Return_Songs_With_Next_Url_If_There_Is_A_Next_Page()
        {
            // Arrange
            var artistName = "que";
            var limit = 1;
            var offset = 0;
            var expectedNextUrl = $"://?artistName={artistName}&offset={offset + limit}&limit={limit}";

            var listSongsMock = new List<SongModel>
            {
                new SongModel { SongId = "songId", SongName = "songName" }
            };

            _songServiceMock
                .GetSongsAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
                .Returns(listSongsMock);

            _songServiceMock
                .GetSongsCountAsync(Arg.Any<string>())
                .Returns(40);

            var songsController = new SongsController(_songServiceMock)
            {
                ControllerContext = new ControllerContext()
            };
            songsController.ControllerContext.HttpContext = new DefaultHttpContext();

            // Act
            var result = await songsController.GetSongs(artistName, limit, offset);

            // Assert
            var objectResultValue = ((OkObjectResult)result.Result).Value;

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            Assert.AreEqual(expectedNextUrl, ((SongListResponseModel)objectResultValue).Next);
        }

        [TestMethod]
        public async Task GetSongs_Should_Return_Songs_With_Next_Url_Null_If_There_Is_Not_A_Next_Page()
        {
            // Arrange
            var artistName = "que";
            var limit = 1;
            var offset = 0;

            var listSongsMock = new List<SongModel>
            {
                new SongModel { SongId = "songId", SongName = "songName" }
            };

            _songServiceMock
                .GetSongsAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>())
                .Returns(listSongsMock);

            _songServiceMock
                .GetSongsCountAsync(Arg.Any<string>())
                .Returns(1);

            var songsController = new SongsController(_songServiceMock)
            {
                ControllerContext = new ControllerContext()
            };
            songsController.ControllerContext.HttpContext = new DefaultHttpContext();

            // Act
            var result = await songsController.GetSongs(artistName, limit, offset);

            // Assert
            var objectResultValue = ((OkObjectResult)result.Result).Value;

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            Assert.IsNull(((SongListResponseModel)objectResultValue).Next);
        }

        [TestMethod]
        public async Task GetSong_Should_Return_NotFound_If_The_Song_Not_Exists()
        {
            // Arrange
            var songId = "songId";

            _songServiceMock
                .GetSongInfoAsync(Arg.Any<string>())
                .Returns((SongInfoModel)null);

            var songsController = new SongsController(_songServiceMock);

            // Act
            var result = await songsController.GetSong(songId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task GetSong_Should_Return_Song_If_The_Song_Exists()
        {
            // Arrange
            var songId = "songId";

            var songInfoModel = new SongInfoModel
            {
                Id = "songIdMock",
                DiscNumber = 2,
                Name = "songName"
            };

            _songServiceMock
                .GetSongInfoAsync(Arg.Any<string>())
                .Returns(songInfoModel);

            var songsController = new SongsController(_songServiceMock);

            // Act
            var result = await songsController.GetSong(songId);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        }
    }
}
