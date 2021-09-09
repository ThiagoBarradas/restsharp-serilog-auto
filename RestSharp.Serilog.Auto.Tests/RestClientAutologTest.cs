using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Xunit;

namespace RestSharp.Serilog.Auto.Tests
{
    public class RestClientAutologTest
    {
        public static readonly string DefaultMessage = "[{Application}] HTTP {Method} {Url} responded {StatusCode} in {ElapsedMilliseconds} ms";

        [Fact]
        public void Should_Construct_With_Empty_Using_Factory()
        {
            // arrange & act
            var restClientAutologFactory = new RestClientFactory();
            var client = (RestClientAutolog) restClientAutologFactory.GetInstance();

            // assert
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForError);
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForSuccess);
            Assert.Null(client.Configuration.LoggerConfiguration);
            Assert.Null(client.BaseUrl);
        }

        [Fact]
        public void Should_Construct_With_BaseUrlAsString_Using_Factory()
        {
            // arrange & act
            var restClientAutologFactory = new RestClientFactory();
            RestClientAutolog client = (RestClientAutolog) restClientAutologFactory.GetInstance("http://www.google.com/");

            // assert
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForError);
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForSuccess);
            Assert.Null(client.Configuration.LoggerConfiguration);
            Assert.Equal("http://www.google.com/", client.BaseUrl.AbsoluteUri);
        }

        [Fact]
        public void Should_Construct_With_GlobalConfiguration()
        {
            // arrange & act
            RestClientAutolog.GlobalConfiguration = new RestClientAutologConfiguration
            {
                MessageTemplateForError = "error",
                MessageTemplateForSuccess = "success"
            };
            var client = new RestClientAutolog("http://www.google.com/");

            RestClientAutolog.GlobalConfiguration.MessageTemplateForError = "new_error";

            // assert
            Assert.Equal("error", client.Configuration.MessageTemplateForError);
            Assert.Equal("success", client.Configuration.MessageTemplateForSuccess);
            Assert.Null(client.Configuration.LoggerConfiguration);
            Assert.Equal("http://www.google.com/", client.BaseUrl.AbsoluteUri);

            // cleanup
            RestClientAutolog.GlobalConfiguration = null;
        }

        [Fact]
        public void Should_Construct_With_BaseUrlAsUri_Using_Factory()
        {
            // arrange & act
            var restClientAutologFactory = new RestClientFactory();
            var client = (RestClientAutolog) restClientAutologFactory.GetInstance(new Uri("http://www.google.com/"));

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
        public void Should_Construct_With_RestClientAutologConfiguration_Using_Factory()
        {
            // arrange & act
            var configuration = new RestClientAutologConfiguration()
            {
                MessageTemplateForError = "Error",
                MessageTemplateForSuccess = "",
                LoggerConfiguration = null
            };

            var restClientAutologFactory = new RestClientFactory();
            var client = (RestClientAutolog)restClientAutologFactory.GetInstance(configuration);

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
        public void Should_Construct_With_BaseUrlAsString_And_RestClientAutologConfiguration_Null()
        {
            // arrange & act
            RestClientAutologConfiguration configuration = null;
            var client = new RestClientAutolog("http://www.github.com/", configuration);

            // assert
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForError);
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForSuccess);
            Assert.Null(client.Configuration.LoggerConfiguration);
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
            var client = new RestClientAutolog("https://reqres.in/api/users/1");
            var restRequest = new RestRequest(Method.GET);
            restRequest.AddQueryParameter("somequery", "test1");
            restRequest.AddQueryParameter("somequery", "test2");
            restRequest.AddQueryParameter("somequery2", "test3");
            restRequest.AddQueryParameter("RequestKey", "123456");
            restRequest.AddQueryParameter("AccountId", "acc_id");

            // act
            var restResponse = client.Execute<User>(restRequest);

            // assert
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForError);
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForSuccess);
            Assert.Null(client.Configuration.LoggerConfiguration);
            Assert.Equal("reqres.in", client.BaseUrl.Host);
            Assert.Equal(1, restResponse.Data.Data.id);
            Assert.Equal("George", restResponse.Data.Data.first_name);
            Assert.Equal(200, (int)restResponse.StatusCode);
            Assert.True(restResponse.IsSuccessful);
        }

        [Fact]
        public void Should_Execute_RestRequest_With_GenericType_And_Success_Async()
        {
            // arrange
            var client = new RestClientAutolog("https://reqres.in/api/users/1");
            var restRequest = new RestRequest(Method.GET);
            restRequest.AddQueryParameter("somequery", "test1");
            restRequest.AddQueryParameter("somequery", "test2");
            restRequest.AddQueryParameter("somequery2", "test3");
            restRequest.AddQueryParameter("RequestKey", "123456");
            restRequest.AddQueryParameter("AccountId", "acc_id");

            // act
            var restResponse = client.ExecuteAsync<User>(restRequest).GetAwaiter().GetResult();

            // assert
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForError);
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForSuccess);
            Assert.Null(client.Configuration.LoggerConfiguration);
            Assert.Equal("reqres.in", client.BaseUrl.Host);
            Assert.Equal(1, restResponse.Data.Data.id);
            Assert.Equal("George", restResponse.Data.Data.first_name);
            Assert.Equal(200, (int)restResponse.StatusCode);
            Assert.True(restResponse.IsSuccessful);
        }

        [Fact]
        public void Should_Execute_RestRequest_With_POST_And_GenericType_And_Success()
        {
            // arrange
            var client = new RestClientAutolog("https://reqres.in/api/users");
            client.Configuration.RequestJsonBlacklist = new string[] { "*job" };
            var restRequest = new RestRequest(Method.POST);
            restRequest.AddHeader("X-Forwarded-For", "127.0.0.1");
            restRequest.AddJsonBody(new
            { 
                name = "Someone",
                job = "Engineer"
            });

            // act
            var restResponse = client.Execute<UserData>(restRequest);

            // assert
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForError);
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForSuccess);
            Assert.Null(client.Configuration.LoggerConfiguration);
            Assert.Equal("reqres.in", client.BaseUrl.Host);
            Assert.Equal("Engineer", restResponse.Data.job);
            Assert.Equal("Someone", restResponse.Data.name);
            Assert.Equal(201, (int)restResponse.StatusCode);
            Assert.True(restResponse.IsSuccessful);
        }

        [Fact]
        public void Should_Execute_RestRequest_With_X_Www_Form_Url_Encoded()
        {
            // arrange
            var client = new RestClientAutolog("http://pruu.herokuapp.com/dump/restsharpAutoLog-test");
            var restRequest = new RestRequest(Method.POST);
            restRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            restRequest.AddHeader("LogIgnored", "ResponseBody,ResponseContent");
            restRequest.AddParameter("", "someproperty=somevalue&someproperty=somevalue2&xpto&xpto=&", ParameterType.RequestBody);

            // act
            var restResponse = client.Execute(restRequest);

            // assert
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForError);
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForSuccess);
            Assert.Null(client.Configuration.LoggerConfiguration);
            Assert.Equal("pruu.herokuapp.com", client.BaseUrl.Host);
            Assert.Equal("OK", restResponse.Content);
            Assert.Equal(200, (int)restResponse.StatusCode);
            Assert.True(restResponse.IsSuccessful);
        }

        [Fact]
        public void Should_Execute_RestRequest_With_X_Www_Form_Url_Encoded_Async()
        {
            // arrange
            IRestClient client = new RestClientAutolog("http://pruu.herokuapp.com/dump/restsharpAutoLog-test");
            var restRequest = new RestRequest(Method.POST);
            restRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            restRequest.AddHeader("LogIgnored", "ResponseBody,ResponseContent");
            restRequest.AddParameter("", "someproperty=somevalue&someproperty=somevalue2&xpto&xpto=&", ParameterType.RequestBody);

            // act
            var restResponse = client.ExecuteAsync(restRequest, Method.POST).GetAwaiter().GetResult();

            // assert
            var clientObj = (RestClientAutolog) client;
            Assert.Equal(DefaultMessage, clientObj.Configuration.MessageTemplateForError);
            Assert.Equal(DefaultMessage, clientObj.Configuration.MessageTemplateForSuccess);
            Assert.Null(clientObj.Configuration.LoggerConfiguration);
            Assert.Equal("pruu.herokuapp.com", client.BaseUrl.Host);
            Assert.Equal("OK", restResponse.Content);
            Assert.Equal(200, (int)restResponse.StatusCode);
            Assert.True(restResponse.IsSuccessful); 
        }

        [Fact]
        public void Should_Execute_RestRequest_Override_LogLevel()
        {
            // arrange
            RestClientAutolog client = new RestClientAutolog("http://pruu.herokuapp.com/dump/restsharpAutoLog-test");
            client.Configuration.OverrideLogLevelByStatusCode = new Dictionary<HttpStatusCode, LogEventLevel>();
            client.Configuration.OverrideLogLevelByStatusCode.Add(HttpStatusCode.OK, LogEventLevel.Debug);
            var restRequest = new RestRequest(Method.POST);
            restRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            restRequest.AddHeader("LogIgnored", "ResponseBody,ResponseContent");
            restRequest.AddParameter("", "someproperty=somevalue&someproperty=somevalue2&xpto&xpto=&", ParameterType.RequestBody);

            // act
            var restResponse = client.ExecuteAsync(restRequest, Method.POST).GetAwaiter().GetResult();

            // assert
            var clientObj = (RestClientAutolog)client;
            Assert.Equal(DefaultMessage, clientObj.Configuration.MessageTemplateForError);
            Assert.Equal(DefaultMessage, clientObj.Configuration.MessageTemplateForSuccess);
            Assert.Null(clientObj.Configuration.LoggerConfiguration);
            Assert.Equal("pruu.herokuapp.com", client.BaseUrl.Host);
            Assert.Equal("OK", restResponse.Content);
            Assert.Equal(200, (int)restResponse.StatusCode);
            Assert.True(restResponse.IsSuccessful);
        }

        [Fact]
        public void Should_Execute_RestRequest_With_Empty_X_Www_Form_Url_Encoded()
        {
            // arrange
            var client = new RestClientAutolog("http://pruu.herokuapp.com/dump/restsharpAutoLog-test");
            var restRequest = new RestRequest(Method.POST);
            restRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            restRequest.AddParameter("", "", ParameterType.RequestBody);

            // act
            var restResponse = client.Execute(restRequest);

            // assert
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForError);
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForSuccess);
            Assert.Null(client.Configuration.LoggerConfiguration);
            Assert.Equal("pruu.herokuapp.com", client.BaseUrl.Host);
            Assert.Equal("OK", restResponse.Content);
            Assert.Equal(200, (int)restResponse.StatusCode);
            Assert.True(restResponse.IsSuccessful);
        }

        [Fact]
        public void Should_Execute_RestRequest_With_InvalidJson()
        {
            // arrange
            var client = new RestClientAutolog("http://pruu.herokuapp.com/dump/restsharpAutoLog-test");
            var restRequest = new RestRequest(Method.POST);
            restRequest.AddHeader("Content-Type", "application/json");
            restRequest.AddParameter("", "{invalid}", ParameterType.RequestBody);

            // act
            var restResponse = client.Execute(restRequest);

            // assert
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForError);
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForSuccess);
            Assert.Null(client.Configuration.LoggerConfiguration);
            Assert.Equal("pruu.herokuapp.com", client.BaseUrl.Host);
            Assert.Equal("OK", restResponse.Content);
            Assert.Equal(200, (int)restResponse.StatusCode);
            Assert.True(restResponse.IsSuccessful);
        }

        [Fact]
        public void Should_Execute_RestRequest_With_ValidJson_OnlyHeader()
        {  
            // arrange
            var client = new RestClientAutolog("http://pruu.herokuapp.com/dump/restsharpAutoLog-test");
            var restRequest = new RestRequest(Method.POST);
            restRequest.AddHeader("Content-Type","application/json");

            //act
            var restResponse = client.Execute(restRequest);

            // assert
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForError);
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForSuccess);
            Assert.Null(client.Configuration.LoggerConfiguration);
            Assert.Equal("pruu.herokuapp.com", client.BaseUrl.Host);
            Assert.Equal(Method.POST, restResponse.Request.Method);
            Assert.Equal(200, (int)restResponse.StatusCode);
            Assert.Equal("OK", restResponse.Content);
            Assert.Single(restResponse.Request.Parameters);
            Assert.True(restResponse.IsSuccessful);
        }

        [Fact]
        public void Should_Execute_RestRequest_With_ValidJson_OnlyRequestBody()
        {
            // arrange
            var client = new RestClientAutolog("http://pruu.herokuapp.com/dump/restsharpAutoLog-test");
            var restRequest = new RestRequest(Method.POST);
            restRequest.AddParameter("Name", "Value", ParameterType.RequestBody);

            // act
            var restResponse = client.Execute(restRequest);

            // assert
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForError);
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForSuccess);
            Assert.Null(client.Configuration.LoggerConfiguration);
            Assert.Equal("pruu.herokuapp.com", client.BaseUrl.Host);
            Assert.Equal(Method.POST, restResponse.Request.Method);
            Assert.Equal(200, (int)restResponse.StatusCode);
            Assert.Equal("OK", restResponse.Content);
            Assert.Single(restResponse.Request.Parameters);
            Assert.True(restResponse.IsSuccessful);
        }
        
        [Fact]
        public void Should_Execute_RestRequest_With_ValidJson_WithoutName()
        {
            // arrange
            var client = new RestClientAutolog("http://pruu.herokuapp.com/dump/restsharpAutoLog-test");
            var restRequest = new RestRequest(Method.POST);
            restRequest.AddParameter("", "Value", ParameterType.RequestBody);

            // act
            var restResponse = client.Execute(restRequest);

            // act
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForError);
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForSuccess);
            Assert.Null(client.Configuration.LoggerConfiguration);
            Assert.Equal("pruu.herokuapp.com", client.BaseUrl.Host);
            Assert.Equal(Method.POST, restResponse.Request.Method);
            Assert.Equal(200, (int)restResponse.StatusCode);
            Assert.Equal("OK", restResponse.Content);
            Assert.Single(restResponse.Request.Parameters);
            Assert.Equal("", restResponse.Request.Parameters.FirstOrDefault().Name);
            Assert.True(restResponse.IsSuccessful);
        }

        [Fact]
        public void Should_Execute_RestRequest_With_ValidJson_Only_With_DataFormat()
        {
            // arrange
            var client = new RestClientAutolog("http://pruu.herokuapp.com/dump/restsharpAutoLog-test");
            var restRequest = new RestRequest(Method.POST);
            restRequest.AddParameter("", "", ParameterType.RequestBody);
            restRequest.Parameters.FirstOrDefault().DataFormat = DataFormat.Json;

            // act
            var restResponse = client.Execute(restRequest);

            // assert
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForError);
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForSuccess);
            Assert.Null(client.Configuration.LoggerConfiguration);
            Assert.Equal("pruu.herokuapp.com", client.BaseUrl.Host);
            Assert.Equal(Method.POST, restResponse.Request.Method);
            Assert.Equal(200, (int)restResponse.StatusCode);
            Assert.Equal("OK", restResponse.Content);
            Assert.Single(restResponse.Request.Parameters);
            Assert.Equal("", restResponse.Request.Parameters.FirstOrDefault().Name);
            Assert.Equal("", restResponse.Request.Parameters.FirstOrDefault().Value);
            Assert.Equal(DataFormat.Json, restResponse.Request.Parameters.FirstOrDefault().DataFormat);
            Assert.True(restResponse.IsSuccessful);
        }

        [Fact]
        public void Should_Execute_RestRequest_With_ValidJson_Only_With_ContentType()
        {
            // arrange
            var client = new RestClientAutolog("http://pruu.herokuapp.com/dump/restsharpAutoLog-test");
            var restRequest = new RestRequest(Method.POST);
            restRequest.AddParameter("", "", ParameterType.RequestBody);
            restRequest.Parameters.FirstOrDefault().ContentType = "application/json";

            // act
            var restResponse = client.Execute(restRequest);

            // assert

            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForError);
            Assert.Equal(DefaultMessage, client.Configuration.MessageTemplateForSuccess);
            Assert.Null(client.Configuration.LoggerConfiguration);
            Assert.Equal("pruu.herokuapp.com", client.BaseUrl.Host);
            Assert.Equal(Method.POST, restResponse.Request.Method);
            Assert.Equal(200, (int)restResponse.StatusCode);
            Assert.Equal("OK", restResponse.Content);
            Assert.Single(restResponse.Request.Parameters);
            Assert.Equal("", restResponse.Request.Parameters.FirstOrDefault().Name);
            Assert.Equal("", restResponse.Request.Parameters.FirstOrDefault().Value);
            Assert.Equal("application/json", restResponse.Request.Parameters.FirstOrDefault().ContentType);
            Assert.True(restResponse.IsSuccessful);
        }
    }

    public class User
    {
        public UserData Data { get; set; }
    }

    public class UserData
    {
        public int id { get; set; }

        public string first_name { get; set; }

        public string last_name { get; set; }

        public string name { get; set; }

        public string job { get; set; }
    }
}
