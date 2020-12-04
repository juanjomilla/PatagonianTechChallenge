using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using NSubstitute;
using PatagonianChallengeAPI.Dao;
using PatagonianChallengeAPI.Dao.Http;
using PatagonianChallengeAPI.Models.Dao.DatabaseModels;

namespace PatagonianChallengeAPI.Tests
{
    [TestClass]
    public class SongsDaoTests
    {
        private static ISongHttpClient _songHttpClientMock;
        private static ISessionFactory _sessionFactory;
        private static IEnumerable<ArtistModel> _artistsMock;

        private static SchemaExport _schemaExport;
        private static SQLiteConnection _connection;

        private const string ConnectionString = "FullUri=file:memorydb.db?mode=memory&cache=shared;DateTimeKind=Utc";

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            var configuration = Fluently.Configure()
                                           .Database(SQLiteConfiguration.Standard.ConnectionString(ConnectionString).ShowSql())
                                           .Mappings(m => m.FluentMappings.AddFromAssemblyOf<ArtistModel>())
                                           .ExposeConfiguration(x => x.SetProperty("current_session_context_class", "call"))
                                           .BuildConfiguration();

            // Create the schema in the database
            // Because it's an in-memory database, we hold this connection open until all the tests are finished
            _schemaExport = new SchemaExport(configuration);
            _connection = new SQLiteConnection(ConnectionString);
            _sessionFactory = configuration.BuildSessionFactory();
        }

        [ClassCleanup]
        public static void BaseClassTearDown()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }

        [TestInitialize]
        public async Task Initialize()
        {
            _connection.Open();
            _schemaExport.Execute(false, true, false, _connection, null);
            _songHttpClientMock = Substitute.For<ISongHttpClient>();
            _artistsMock = GetEntitiesFromJSONFile<ArtistModel>(Path.Combine(Directory.GetCurrentDirectory(), "Entities\\SongEntities.json"));

            await SaveEntitiesIntoDatabase(_artistsMock);
        }

        [TestCleanup]
        public void TearDown()
        {
            if (_connection != null)
            {
                _connection.Close();
            }
        }

        [TestMethod]
        public async Task GetSongsByArtistNameAsync_With_Limit_Should_Return_Songs_By_Artist_Name_With_Limit()
        {
            // Arrange
            var limit = 2;
            var songsDao = new SongsDao(_sessionFactory, _songHttpClientMock);

            // Act
            var songs = await songsDao.GetSongsByArtistNameAsync("Daft", limit, 1);

            // Assert
            Assert.IsNotNull(songs);
            Assert.IsInstanceOfType(songs, typeof(IEnumerable<SongModel>));
            Assert.AreEqual(limit, songs.Count());
        }

        [TestMethod]
        public async Task GetSongsByArtistNameAsync_With_Offset_Should_Return_Songs_By_Artist_Name_With_Offset()
        {
            // Arrange
            var offset = 1;
            var expectedSongId = "secondSong";
            var songsDao = new SongsDao(_sessionFactory, _songHttpClientMock);

            // Act
            var songs = await songsDao.GetSongsByArtistNameAsync("Daft", 1, offset);

            // Assert
            Assert.IsNotNull(songs);
            Assert.IsInstanceOfType(songs, typeof(IEnumerable<SongModel>));
            Assert.AreEqual(expectedSongId, songs.First().SongId);
        }

        [TestMethod]
        public async Task GetSongsByArtistNameAsync_With_No_Existent_Artist_Should_Return_Empty_IEnumerable()
        {
            // Arrange
            var songsDao = new SongsDao(_sessionFactory, _songHttpClientMock);

            // Act
            var songs = await songsDao.GetSongsByArtistNameAsync("NoExistentArtist", 20, 20);

            // Assert
            Assert.IsNotNull(songs);
            Assert.IsInstanceOfType(songs, typeof(IEnumerable<SongModel>));
            Assert.AreEqual(0, songs.Count());
        }

        [TestMethod]
        public async Task GetSongsCountAsync_With_No_Existent_Artist_Should_Return_0()
        {
            // Arrange
            var songsDao = new SongsDao(_sessionFactory, _songHttpClientMock);

            // Act
            var songsCount = await songsDao.GetSongsCountAsync("NoExistentArtist");

            // Assert
            Assert.IsInstanceOfType(songsCount, typeof(int));
            Assert.AreEqual(0, songsCount);
        }

        [TestMethod]
        public async Task GetSongsCountAsync_With_Existent_Artist_Should_Return_Total_Songs()
        {
            // Arrange
            var songsDao = new SongsDao(_sessionFactory, _songHttpClientMock);

            // Act
            var songsCount = await songsDao.GetSongsCountAsync("Daft");

            // Assert
            Assert.IsInstanceOfType(songsCount, typeof(int));
            Assert.AreEqual(4, songsCount);
        }

        [TestMethod]
        public async Task GetSongByIdAsync_With_Existent_Song_Should_Return_Song()
        {
            // Arrange
            var expectedSongId = "fourthSong";
            var songsDao = new SongsDao(_sessionFactory, _songHttpClientMock);

            // Act
            var songModel = await songsDao.GetSongByIdAsync("fourthSong");

            // Assert
            Assert.IsInstanceOfType(songModel, typeof(SongModel));
            Assert.AreEqual(expectedSongId, songModel.SongId);
        }

        [TestMethod]
        public async Task GetSongByIdAsync_With_No_Existent_Song_Should_Return_Null()
        {
            // Arrange
            var songsDao = new SongsDao(_sessionFactory, _songHttpClientMock);

            // Act
            var songModel = await songsDao.GetSongByIdAsync("noExistentSongId");

            // Assert
            Assert.AreEqual(null, songModel);
        }

        private async Task SaveEntitiesIntoDatabase<T>(IEnumerable<T> entities)
        {
            using (var session = _sessionFactory.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                foreach (var entity in entities)
                {
                    await session.PersistAsync(entity);
                }

                await tx.CommitAsync();
            }
        }

        private IEnumerable<T> GetEntitiesFromJSONFile<T>(string entitiesJsonPath)
        {
            IEnumerable<T> entities;

            // reads a JSON file from the file test
            using var file = File.OpenText(entitiesJsonPath);
            using var reader = new JsonTextReader(file);
            
            entities = ((JArray)JToken.ReadFrom(reader)).ToObject<IEnumerable<T>>();

            return entities;
        }
    }
}
