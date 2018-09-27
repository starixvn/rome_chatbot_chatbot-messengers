namespace Messenger.Client.Tests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Messenger.Client.Configurations;
    using Messenger.Client.Helpers;
    using NSubstitute;
    using RestSharp;
    using Xunit;

    public class RestClientHelperTests
    {
        private readonly IRestClient restClient;
        private readonly IJwtHelper jwtHelper;
        private readonly IMessengerSettings messengerSettings;
        private readonly RestClientHelper restClientHelper;

        public RestClientHelperTests()
        {
            restClient = Substitute.For<IRestClient>();
            jwtHelper = Substitute.For<IJwtHelper>();
            messengerSettings = Substitute.For<IMessengerSettings>();
            restClientHelper = new RestClientHelper(restClient, jwtHelper, messengerSettings);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ExecuteGet_InvalidEndPoint_ThrowsArgumentNullException(string endPoint)
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => restClientHelper.ExecuteGet<object>(endPoint));
        }

        [Fact]
        public void ExecuteGet_ValidEndPoint_CallsRestClientExecute()
        {
            // Arrange
            var endPoint = "endpoint";

            var response = Substitute.For<IRestResponse<object>>();
            response.IsSuccessful.Returns(true);
            restClient.Execute<object>(Arg.Any<IRestRequest>()).Returns(response);

            // Act
            restClientHelper.ExecuteGet<object>(endPoint);

            // Assert
            restClient.Received().Execute<object>(
                Arg.Is<IRestRequest>(
                    r => r.Resource.Equals(endPoint)
                        && r.Method == Method.GET));
        }

        [Fact]
        public void ExecuteGet_FailedResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var endPoint = "endpoint";

            var response = Substitute.For<IRestResponse<object>>();
            response.IsSuccessful.Returns(false);
            restClient.Execute<object>(Arg.Any<IRestRequest>()).Returns(response);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => restClientHelper.ExecuteGet<object>(endPoint));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ExecutePost_InvalidEndPoint_ThrowsArgumentNullException(string endPoint)
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => restClientHelper.ExecutePost<object>(endPoint, new Dictionary<string, object>()));
        }

        [Fact]
        public void ExecutePost_ValidEndPoint_CallsRestClientExecute()
        {
            // Arrange
            var endPoint = "endpoint";
            var data = new Dictionary<string, object>();

            var response = Substitute.For<IRestResponse<object>>();
            response.IsSuccessful.Returns(true);
            restClient.Execute<object>(Arg.Any<IRestRequest>()).Returns(response);

            // Act
            restClientHelper.ExecutePost<object>(endPoint, data);

            // Assert
            restClient.Received().Execute<object>(
                Arg.Is<IRestRequest>(
                    r => r.Resource.Equals(endPoint)
                        && r.Method == Method.POST));
        }

        [Fact]
        public void ExecutePost_FailedResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var endPoint = "endpoint";

            var response = Substitute.For<IRestResponse<object>>();
            response.IsSuccessful.Returns(false);
            restClient.Execute<object>(Arg.Any<IRestRequest>()).Returns(response);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(
                () => restClientHelper.ExecutePost<object>(endPoint, new Dictionary<string, object>()));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ExecutePut_InvalidEndPoint_ThrowsArgumentNullException(string endPoint)
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => restClientHelper.ExecutePut<object>(endPoint, new Dictionary<string, object>()));
        }

        [Fact]
        public void ExecutePut_ValidEndPoint_CallsRestClientExecute()
        {
            // Arrange
            var endPoint = "endpoint";
            var data = new Dictionary<string, object>();

            var response = Substitute.For<IRestResponse<object>>();
            response.IsSuccessful.Returns(true);
            restClient.Execute<object>(Arg.Any<IRestRequest>()).Returns(response);

            // Act
            restClientHelper.ExecutePut<object>(endPoint, data);

            // Assert
            restClient.Received().Execute<object>(
                Arg.Is<IRestRequest>(
                    r => r.Resource.Equals(endPoint)
                        && r.Method == Method.PUT));
        }

        [Fact]
        public void ExecutePut_FailedResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var endPoint = "endpoint";

            var response = Substitute.For<IRestResponse<object>>();
            response.IsSuccessful.Returns(false);
            restClient.Execute<object>(Arg.Any<IRestRequest>()).Returns(response);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(
                () => restClientHelper.ExecutePut<object>(endPoint, new Dictionary<string, object>()));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ExecuteDelete_InvalidEndPoint_ThrowsArgumentNullException(string endPoint)
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () => restClientHelper.ExecuteDelete<object>(endPoint, new Dictionary<string, object>()));
        }

        [Fact]
        public void ExecuteDelete_ValidEndPoint_CallsRestClientExecute()
        {
            // Arrange
            var endPoint = "endpoint";

            var response = Substitute.For<IRestResponse<object>>();
            response.IsSuccessful.Returns(true);
            restClient.Execute<object>(Arg.Any<IRestRequest>()).Returns(response);

            // Act
            restClientHelper.ExecuteDelete<object>(endPoint, new Dictionary<string, object>());

            // Assert
            restClient.Received().Execute<object>(
                Arg.Is<IRestRequest>(
                    r => r.Resource.Equals(endPoint)
                        && r.Method == Method.DELETE));
        }

        [Fact]
        public void ExecuteDelete_FailedResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var endPoint = "endpoint";

            var response = Substitute.For<IRestResponse<object>>();
            response.IsSuccessful.Returns(false);
            restClient.Execute<object>(Arg.Any<IRestRequest>()).Returns(response);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(
                () => restClientHelper.ExecuteDelete<object>(endPoint, new Dictionary<string, object>()));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ExecuteGetAsync_InvalidEndPoint_ThrowsArgumentNullException(string endPoint)
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => restClientHelper.ExecuteGetAsync<object>(endPoint));
        }

        [Fact]
        public void ExecuteGetAsync_ValidEndPoint_CallsRestClientExecute()
        {
            // Arrange
            var endPoint = "endpoint";

            var response = Substitute.For<IRestResponse<object>>();
            response.IsSuccessful.Returns(true);
            restClient.ExecuteTaskAsync<object>(Arg.Any<IRestRequest>())
                .Returns(Task.FromResult(response));

            // Act
            restClientHelper.ExecuteGetAsync<object>(endPoint);

            // Assert
            restClient.Received().ExecuteTaskAsync<object>(
                Arg.Is<IRestRequest>(
                    r => r.Resource.Equals(endPoint)
                        && r.Method == Method.GET));
        }

        [Fact]
        public void ExecuteGetAsync_FailedResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var endPoint = "endpoint";

            var response = Substitute.For<IRestResponse<object>>();
            response.IsSuccessful.Returns(false);
            restClient.ExecuteTaskAsync<object>(Arg.Any<IRestRequest>())
                .Returns(Task.FromResult(response));

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(
                () => restClientHelper.ExecuteGetAsync<object>(endPoint));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ExecutePostAsync_InvalidEndPoint_ThrowsArgumentNullException(string endPoint)
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(
                () => restClientHelper.ExecutePostAsync<object>(endPoint, new Dictionary<string, object>()));
        }

        [Fact]
        public void ExecutePostAsync_ValidEndPoint_CallsRestClientExecute()
        {
            // Arrange
            var endPoint = "endpoint";

            var response = Substitute.For<IRestResponse<object>>();
            response.IsSuccessful.Returns(true);
            restClient.ExecuteTaskAsync<object>(Arg.Any<IRestRequest>())
                .Returns(Task.FromResult(response));

            // Act
            restClientHelper.ExecutePostAsync<object>(endPoint, new Dictionary<string, object>());

            // Assert
            restClient.Received().ExecuteTaskAsync<object>(
                Arg.Is<IRestRequest>(
                    r => r.Resource.Equals(endPoint)
                        && r.Method == Method.POST));
        }

        [Fact]
        public void ExecutePostAsync_FailedResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var endPoint = "endpoint";

            var response = Substitute.For<IRestResponse<object>>();
            response.IsSuccessful.Returns(false);
            restClient.ExecuteTaskAsync<object>(Arg.Any<IRestRequest>())
                .Returns(Task.FromResult(response));

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(
                () => restClientHelper.ExecutePostAsync<object>(endPoint, new Dictionary<string, object>()));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ExecutePutAsync_InvalidEndPoint_ThrowsArgumentNullException(string endPoint)
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(
                () => restClientHelper.ExecutePutAsync<object>(endPoint, new Dictionary<string, object>()));
        }

        [Fact]
        public void ExecutePutAsync_ValidEndPoint_CallsRestClientExecute()
        {
            // Arrange
            var endPoint = "endpoint";

            var response = Substitute.For<IRestResponse<object>>();
            response.IsSuccessful.Returns(true);
            restClient.ExecuteTaskAsync<object>(Arg.Any<IRestRequest>())
                .Returns(Task.FromResult(response));

            // Act
            restClientHelper.ExecutePutAsync<object>(endPoint, new Dictionary<string, object>());

            // Assert
            restClient.Received().ExecuteTaskAsync<object>(
                Arg.Is<IRestRequest>(
                    r => r.Resource.Equals(endPoint)
                        && r.Method == Method.PUT));
        }

        [Fact]
        public void ExecutePutAsync_FailedResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var endPoint = "endpoint";

            var response = Substitute.For<IRestResponse<object>>();
            response.IsSuccessful.Returns(false);
            restClient.ExecuteTaskAsync<object>(Arg.Any<IRestRequest>())
                .Returns(Task.FromResult(response));

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(
                () => restClientHelper.ExecutePutAsync<object>(endPoint, new Dictionary<string, object>()));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ExecuteDeleteAsync_InvalidEndPoint_ThrowsArgumentNullException(string endPoint)
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(
                () => restClientHelper.ExecuteDeleteAsync<object>(endPoint, new Dictionary<string, object>()));
        }

        [Fact]
        public void ExecuteDeleteAsync_ValidEndPoint_CallsRestClientExecute()
        {
            // Arrange
            var endPoint = "endpoint";

            var response = Substitute.For<IRestResponse<object>>();
            response.IsSuccessful.Returns(true);
            restClient.ExecuteTaskAsync<object>(Arg.Any<IRestRequest>())
                .Returns(Task.FromResult(response));

            // Act
            restClientHelper.ExecuteDeleteAsync<object>(endPoint, new Dictionary<string, object>());

            // Assert
            restClient.Received().ExecuteTaskAsync<object>(
                Arg.Is<IRestRequest>(
                    r => r.Resource.Equals(endPoint)
                        && r.Method == Method.DELETE));
        }

        [Fact]
        public void ExecuteDeleteAsync_FailedResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            var endPoint = "endpoint";

            var response = Substitute.For<IRestResponse<object>>();
            response.IsSuccessful.Returns(false);
            restClient.ExecuteTaskAsync<object>(Arg.Any<IRestRequest>())
                .Returns(Task.FromResult(response));

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(
                () => restClientHelper.ExecuteDeleteAsync<object>(endPoint, new Dictionary<string, object>()));
        }
    }
}