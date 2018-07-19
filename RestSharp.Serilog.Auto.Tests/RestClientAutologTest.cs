using Serilog;
using System;
using Xunit;

namespace RestSharp.Serilog.Auto.Tests
{
    public class RestClientAutologTest
    {
        public static string DefaultMessage = "[{Application}] HTTP {Method} {Uri} responded {StatusCode} in {ElapsedMilliseconds} ms";

        [Fact]
        public void Should_Construct_With_Empty()
        {
            // arrange & act
            var client = new RestClientAutolog();

            // assert
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForError);
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForSuccess);
            Assert.Null(client.Configuration.LoggerConfiguration);
            Assert.Null(client.BaseUrl);
        }

        [Fact]
        public void Should_Construct_With_BaseUrlAsString()
        {
            // arrange & act
            var client = new RestClientAutolog("http://www.google.com/");

            // assert
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForError);
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForSuccess);
            Assert.Null(client.Configuration.LoggerConfiguration);
            Assert.Equal("http://www.google.com/", client.BaseUrl.AbsoluteUri);
        }

        [Fact]
        public void Should_Construct_With_BaseUrlAsUri()
        {
            // arrange & act
            var client = new RestClientAutolog(new Uri("http://www.google.com/"));

            // assert
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForError);
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForSuccess);
            Assert.Null(client.Configuration.LoggerConfiguration);
            Assert.Equal("http://www.google.com/", client.BaseUrl.AbsoluteUri);
        }

        [Fact]
        public void Should_Construct_With_BaseUrlAsString_And_DefaultMessage()
        {
            // arrange & act
            var client = new RestClientAutolog("http://www.google.com/", "Message");

            // assert
            Assert.Equal("Message", client.Configuration.MessageTemplateForError);
            Assert.Equal("Message", client.Configuration.MessageTemplateForSuccess);
            Assert.Null(client.Configuration.LoggerConfiguration);
            Assert.Equal("http://www.google.com/", client.BaseUrl.AbsoluteUri);
        }

        [Fact]
        public void Should_Construct_With_LoggerConfiguration()
        {
            // arrange & act
            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console();
            var client = new RestClientAutolog(loggerConfiguration);

            // assert
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForError);
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForSuccess);
            Assert.NotNull(client.Configuration.LoggerConfiguration);
            Assert.Null(client.BaseUrl);
        }

        [Fact]
        public void Should_Construct_With_BaseUrlAsString_And_LoggerConfiguration()
        {
            // arrange & act
            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console();
            var client = new RestClientAutolog("http://www.google.com/", loggerConfiguration);

            // assert
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForError);
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForSuccess);
            Assert.NotNull(client.Configuration.LoggerConfiguration);
            Assert.Equal("http://www.google.com/", client.BaseUrl.AbsoluteUri);
        }

        [Fact]
        public void Should_Construct_With_BaseUrlAsUri_And_LoggerConfiguration()
        {
            // arrange & act
            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console();
            var client = new RestClientAutolog(new Uri("http://www.google.com/"), loggerConfiguration);

            // assert
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForError);
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForSuccess);
            Assert.NotNull(client.Configuration.LoggerConfiguration);
            Assert.Equal("http://www.google.com/", client.BaseUrl.AbsoluteUri);
        }

        [Fact]
        public void Should_Construct_With_RestClientAutologConfiguration()
        {
            // arrange & act
            var configuration = new RestClientAutologConfiguration()
            {
                MessageTemplateForError = "Error",
                MessageTemplateForSuccess = "",
                LoggerConfiguration = null
            };
            var client = new RestClientAutolog(configuration);

            // assert
            Assert.Equal("Error", client.Configuration.MessageTemplateForError);
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForSuccess);
            Assert.Null(client.Configuration.LoggerConfiguration);
            Assert.Null(client.BaseUrl);
        }

        [Fact]
        public void Should_Construct_With_BaseUrlAsString_And_RestClientAutologConfiguration()
        {
            // arrange & act
            var configuration = new RestClientAutologConfiguration()
            {
                MessageTemplateForError = "",
                MessageTemplateForSuccess = "Success",
                LoggerConfiguration = new LoggerConfiguration()
            };
            var client = new RestClientAutolog("http://www.github.com/", configuration);

            // assert
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForError);
            Assert.Equal("Success", client.Configuration.MessageTemplateForSuccess);
            Assert.NotNull(client.Configuration.LoggerConfiguration);
            Assert.Equal("http://www.github.com/", client.BaseUrl.AbsoluteUri);
        }

        [Fact]
        public void Should_Construct_With_BaseUrlAsUri_And_RestClientAutologConfiguration()
        {
            // arrange & act
            var configuration = new RestClientAutologConfiguration()
            {
                MessageTemplateForError = "",
                MessageTemplateForSuccess = null,
                LoggerConfiguration = new LoggerConfiguration()
            };
            var client = new RestClientAutolog(new Uri("http://www.github.com/"), configuration);

            // assert
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForError);
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForSuccess);
            Assert.NotNull(client.Configuration.LoggerConfiguration);
            Assert.Equal("http://www.github.com/", client.BaseUrl.AbsoluteUri);
        }

        [Fact]
        public void Should_Execute_RestRequest_With_Error()
        {
            // arrange
            var configuration = new RestClientAutologConfiguration()
            {
                MessageTemplateForError = "Error",
                MessageTemplateForSuccess = "Success",
                LoggerConfiguration = new LoggerConfiguration()
            };
            var client = new RestClientAutolog("http://www.bing.com/", configuration)
            {
                Timeout = 1
            };
            var restRequest = new RestRequest(Method.GET);

            // act
            var restResponse = client.Execute(restRequest);

            // assert
            Assert.Equal("Error", client.Configuration.MessageTemplateForError);
            Assert.Equal("Success", client.Configuration.MessageTemplateForSuccess);
            Assert.NotNull(client.Configuration.LoggerConfiguration);
            Assert.Equal("http://www.bing.com/", client.BaseUrl.AbsoluteUri);
            Assert.NotNull(restResponse.ErrorException);
            Assert.NotNull(restResponse.ErrorMessage);
            Assert.False(restResponse.IsSuccessful);
        }

        [Fact]
        public void Should_Execute_RestRequest_With_GenericType_And_Success()
        {
            // arrange
            var client = new RestClientAutolog("http://md5.jsontest.com/");
            var restRequest = new RestRequest(Method.GET);
            restRequest.AddQueryParameter("text", "Thiago Barradas");

            // act
            var restResponse = client.Execute<MD5>(restRequest);

            // assert
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForError);
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForSuccess);
            Assert.Null(client.Configuration.LoggerConfiguration);
            Assert.Equal("http://md5.jsontest.com/", client.BaseUrl.AbsoluteUri);
            Assert.Equal("Thiago Barradas", restResponse.Data.Original);
            Assert.Equal("ad5879ef35a1e0156d8cb4809df3d69d", restResponse.Data.Md5);
            Assert.Equal(200, (int)restResponse.StatusCode);
            Assert.True(restResponse.IsSuccessful);
        }
    }

    public class MD5
    {
        public string Md5 { get; set; }

        public string Original { get; set; }
    }
}
