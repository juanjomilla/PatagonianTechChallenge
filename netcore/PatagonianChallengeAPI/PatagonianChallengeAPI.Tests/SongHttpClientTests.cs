using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using PatagonianChallengeAPI.Dao.Http;
using PatagonianChallengeAPI.Tests.Mocks;

namespace PatagonianChallengeAPI.Tests
{
    [TestClass]
    public class SongHttpClientTests
    {
        private static IHttpClientFactory _httpClientFactoryMock;
        private static IConfiguration _configurationMock;

        [TestInitialize]
        public void Initialize()
        {
            _httpClientFactoryMock = Substitute.For<IHttpClientFactory>();
            _configurationMock = Substitute.For<IConfiguration>();
        }

        [TestMethod]
        public async Task GetSongInfoAsync_Should_Deserialize_Song_From_API()
        {
            // Arrange
            var messageHandlerMock = new MockHttpMessageHandler((request, cancellation) => 
            {
                var responseMessage =
                    new System.Net.Http.HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new System.Net.Http.StringContent("{\"disc_number\": 1, \"name\": \"testName\", \"id\": \"testId\"}")
                    };

                var result = Task.FromResult(responseMessage);
                return result;
            });

            var authMessageHandlerMock = new MockHttpMessageHandler((request, cancellation) =>
            {
                var responseMessage =
                    new System.Net.Http.HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new System.Net.Http.StringContent("{\"access_token\": \"accessTokenMock\"}")
                    };

                var result = Task.FromResult(responseMessage);
                return result;
            });

            var httpClient = new System.Net.Http.HttpClient(messageHandlerMock);
            var authHttpClient = new System.Net.Http.HttpClient(authMessageHandlerMock);
            _httpClientFactoryMock.Create().Returns(x => httpClient, x => authHttpClient);

            var songHttpClient = new SongHttpClient(_httpClientFactoryMock, _configurationMock);

            // Act
            var songInfo = await songHttpClient.GetSongInfoAsync("songIdTest");

            // Assert
            Assert.IsNotNull(songInfo);
            Assert.AreEqual(1, songInfo.DiscNumber);
            Assert.AreEqual("testName", songInfo.Name);
            Assert.AreEqual("testId", songInfo.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(System.Net.Http.HttpRequestException), "No exception thrown when getting auth token failed")]
        public async Task GetSongInfoAsync_Should_Throw_An_Exception_If_An_Error_Ocurred_When_Getting_Auth_Token()
        {
            // Arrange
            var authMessageHandlerMock = new MockHttpMessageHandler((request, cancellation) =>
            {
                var responseMessage =
                    new System.Net.Http.HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new System.Net.Http.StringContent("{}")
                    };

                var result = Task.FromResult(responseMessage);
                return result;
            });

            var authHttpClient = new System.Net.Http.HttpClient(authMessageHandlerMock);
            _httpClientFactoryMock.Create().Returns(x => authHttpClient);

            var songHttpClient = new SongHttpClient(_httpClientFactoryMock, _configurationMock);

            // Act
            var songInfo = await songHttpClient.GetSongInfoAsync("songIdTest");
        }

        [TestMethod]
        [ExpectedException(typeof(System.Net.Http.HttpRequestException), "No exception thrown when getting auth token failed")]
        public async Task GetSongInfoAsync_Should_Throw_An_Exception_If_Get_Track_Endpoint_Returns_BadRequest()
        {
            // Arrange
            var messageHandlerMock = new MockHttpMessageHandler((request, cancellation) =>
            {
                var responseMessage =
                    new System.Net.Http.HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new System.Net.Http.StringContent("{\"disc_number\": 1, \"name\": \"testName\", \"id\": \"testId\"}")
                    };

                var result = Task.FromResult(responseMessage);
                return result;
            });

            var authMessageHandlerMock = new MockHttpMessageHandler((request, cancellation) =>
            {
                var responseMessage =
                    new System.Net.Http.HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new System.Net.Http.StringContent("{\"access_token\": \"accessTokenMock\"}")
                    };

                var result = Task.FromResult(responseMessage);
                return result;
            });

            var httpClient = new System.Net.Http.HttpClient(messageHandlerMock);
            var authHttpClient = new System.Net.Http.HttpClient(authMessageHandlerMock);
            _httpClientFactoryMock.Create().Returns(x => httpClient, x => authHttpClient);

            var songHttpClient = new SongHttpClient(_httpClientFactoryMock, _configurationMock);

            // Act
            var songInfo = await songHttpClient.GetSongInfoAsync("songIdTest");
        }
    }
}
