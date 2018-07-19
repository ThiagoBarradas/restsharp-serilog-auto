using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RestSharp
{
    public class RestClientAutolog : RestClient
    {
        public RestClientAutologConfiguration Configuration { get; set; }

        public RestClientAutolog(RestClientAutologConfiguration configuration)
        {
            this.Startup(configuration);
        }

        public RestClientAutolog(Uri baseUrl, RestClientAutologConfiguration configuration) : base(baseUrl)
        {
            this.Startup(configuration);
        }

        public RestClientAutolog(string baseUrl, RestClientAutologConfiguration configuration) : base(baseUrl)
        {
            this.Startup(configuration);
        }

        public RestClientAutolog(LoggerConfiguration loggerConfiguration)
        {
            this.Startup(new RestClientAutologConfiguration()
            {
                LoggerConfiguration = loggerConfiguration
            });
        }

        public RestClientAutolog(Uri baseUrl, LoggerConfiguration loggerConfiguration) : base(baseUrl)
        {
            this.Startup(new RestClientAutologConfiguration()
            {
                LoggerConfiguration = loggerConfiguration
            });
        }

        public RestClientAutolog(string baseUrl, LoggerConfiguration loggerConfiguration) : base(baseUrl)
        {
            this.Startup(new RestClientAutologConfiguration()
            {
                LoggerConfiguration = loggerConfiguration
            });
        }

        public RestClientAutolog(string baseUrl, string message) : base(baseUrl)
        {
            this.Startup(new RestClientAutologConfiguration()
            {
                MessageTemplateForError = message,
                MessageTemplateForSuccess = message
            });
        }

        public RestClientAutolog(string baseUrl) : base(baseUrl)
        {
            this.Startup(new RestClientAutologConfiguration());
        }

        public RestClientAutolog(Uri baseUrl) : base(baseUrl)
        {
            this.Startup(new RestClientAutologConfiguration());
        }

        public RestClientAutolog()
        {
            this.Startup(new RestClientAutologConfiguration());
        }

        private void Startup(RestClientAutologConfiguration configuration)
        {
            if (configuration == null)
            {
                configuration = new RestClientAutologConfiguration();
            }

            this.Configuration = configuration;
        }

        public override IRestResponse Execute(IRestRequest request)
        {
            var stopwatch = Stopwatch.StartNew();

            var response = base.Execute(request);
            stopwatch.Stop(); 

            this.LogRequestAndResponse(response, stopwatch);

            return response;
        }

        public override IRestResponse<T> Execute<T>(IRestRequest request)
        {
            var stopwatch = Stopwatch.StartNew();

            var response = base.Execute<T>(request);
            stopwatch.Stop();

            this.LogRequestAndResponse(response, stopwatch);

            return response;
        }

        private void LogRequestAndResponse(IRestResponse response, Stopwatch stopwatch)
        {
            if (this.Configuration.LoggerConfiguration != null)
            {
                Log.Logger = this.Configuration.LoggerConfiguration.CreateLogger();
            }

            var uri = this.BuildUri(response.Request);
            LogContext.PushProperty("Agent", "RestSharp");
            LogContext.PushProperty("ElapsedMilliseconds", stopwatch.ElapsedMilliseconds);
            LogContext.PushProperty("Method", response.Request?.Method.ToString());
            LogContext.PushProperty("Uri", uri.AbsoluteUri);
            LogContext.PushProperty("Host", uri.Host);
            LogContext.PushProperty("Path", uri.AbsolutePath);
            LogContext.PushProperty("Query", this.GetRequestQueryString(response.Request));
            LogContext.PushProperty("Body", this.GetRequestBody(response.Request));
            LogContext.PushProperty("RequestHeaders", this.GetRequestHeaders(response.Request));
            LogContext.PushProperty("StatusCode", (int)response.StatusCode);
            LogContext.PushProperty("StatusDescription", response.StatusDescription);
            LogContext.PushProperty("ResponseStatus", response.ResponseStatus.ToString());
            LogContext.PushProperty("ProtocolVersion", response.ProtocolVersion);
            LogContext.PushProperty("IsSuccessful", response.IsSuccessful);
            LogContext.PushProperty("ErrorMessage", response.ErrorMessage);
            LogContext.PushProperty("ErrorException", response.ErrorException);
            LogContext.PushProperty("Content", response.Content);
            LogContext.PushProperty("ContentEncoding", response.ContentEncoding);
            LogContext.PushProperty("ContentLength", response.ContentLength);
            LogContext.PushProperty("ContentType", response.ContentType);
            LogContext.PushProperty("ResponseHeaders", response.Headers);

            if (response.IsSuccessful)
            {
                Log.Information(this.Configuration.MessageTemplateForSuccess);
            }
            else
            {
                Log.Error(this.Configuration.MessageTemplateForSuccess);
            }
        }

        private object GetRequestBody(IRestRequest request)
        {
            var body = request?.Parameters?.FirstOrDefault(p => p.Type == ParameterType.RequestBody);
            return body?.Value;
        }

        private IEnumerable<Parameter> GetRequestHeaders(IRestRequest request)
        {
            var query = request?.Parameters?.Where(p => p.Type == ParameterType.HttpHeader);
            return query;
        }

        private IEnumerable<Parameter> GetRequestQueryString(IRestRequest request)
        {
            var headers = request?.Parameters?.Where(p => p.Type == ParameterType.QueryString);
            return headers;
        }
    }
}