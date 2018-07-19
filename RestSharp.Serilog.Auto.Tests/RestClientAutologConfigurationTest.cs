using Serilog;
using System;
using Xunit;

namespace RestSharp.Serilog.Auto.Tests
{
    public class RestClientAutologConfigurationTest
    {
        public static string DefaultMessage = "[{Application}] HTTP {Method} {Uri} responded {StatusCode} in {ElapsedMilliseconds} ms";

        [Fact]
        public void Should_Return_Default_Values()
        {
            // arrange & act
            var config = new RestClientAutologConfiguration();

            // assert
            Assert.Equal(DefaultMessage, config.MessageTemplateForError);
            Assert.Equal(DefaultMessage, config.MessageTemplateForSuccess);
            Assert.Null(config.LoggerConfiguration);
        }

        [Fact]
        public void Should_Return_New_Values()
        {
            // arrange & act
            var config = new RestClientAutologConfiguration();
            config.MessageTemplateForError = "Error";
            config.MessageTemplateForSuccess = "Success";
            config.LoggerConfiguration = new LoggerConfiguration();

            // assert
            Assert.Equal("Error", config.MessageTemplateForError);
            Assert.Equal("Success", config.MessageTemplateForSuccess);
            Assert.NotNull(config.LoggerConfiguration);
        }

        [Fact]
        public void Should_Return_Default_Values_When_String_Empty()
        {
            // arrange & act
            var config = new RestClientAutologConfiguration();
            config.MessageTemplateForError = "";
            config.MessageTemplateForError = "";

            // assert
            Assert.Equal(DefaultMessage, config.MessageTemplateForError);
            Assert.Equal(DefaultMessage, config.MessageTemplateForSuccess);
        }
    }
}
