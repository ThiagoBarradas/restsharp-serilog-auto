using JsonMasking;
using PackUtils;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

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

        private void LogRequestAndResponse(IRestResponse response, Stopwatch stopwatch)
        {
            if (this.Configuration.LoggerConfiguration != null)
            {
                Log.Logger = this.Configuration.LoggerConfiguration.CreateLogger();
            }

            var uri = this.BuildUri(response.Request);
            LogContext.PushProperty("Agent", "RestSharp");
            LogContext.PushProperty("ElapsedMilliseconds", stopwatch.ElapsedMilliseconds);
            LogContext.PushProperty("Method", response.Request.Method.ToString());
            LogContext.PushProperty("Url", uri.AbsoluteUri);
            LogContext.PushProperty("Host", uri.Host);
            LogContext.PushProperty("Path", uri.AbsolutePath);
            LogContext.PushProperty("Port", uri.Port);
            LogContext.PushProperty("RequestKey", this.GetRequestKey(response.Request));
            LogContext.PushProperty("QueryString", uri.Query);
            LogContext.PushProperty("Query", this.GetRequestQueryStringAsObject(response.Request));
            LogContext.PushProperty("RequestBody", this.GetRequestBody(response.Request));
            LogContext.PushProperty("RequestHeaders", this.GetRequestHeaders(response.Request));
            LogContext.PushProperty("StatusCode", (int)response.StatusCode);
            LogContext.PushProperty("StatusCodeFamily", ((int)response.StatusCode).ToString()[0] + "XX");
            LogContext.PushProperty("StatusDescription", response.StatusDescription?.Replace(" ",""));
            LogContext.PushProperty("ResponseStatus", response.ResponseStatus.ToString());
            LogContext.PushProperty("ProtocolVersion", response.ProtocolVersion);
            LogContext.PushProperty("IsSuccessful", response.IsSuccessful);
            LogContext.PushProperty("ErrorMessage", response.ErrorMessage);
            LogContext.PushProperty("ErrorException", response.ErrorException);
            LogContext.PushProperty("ResponseContent", this.GetResponseContent(response));
            LogContext.PushProperty("ContentLength", response.ContentLength);
            LogContext.PushProperty("ContentType", response.ContentType);
            LogContext.PushProperty("ResponseHeaders", this.GetResponseHeaders(response));

            if (response.IsSuccessful)
            {
                Log.Information(this.Configuration.MessageTemplateForSuccess);
            }
            else
            {
                Log.Error(this.Configuration.MessageTemplateForSuccess);
            }
        }

        private object GetRequestKey(IRestRequest request)
        {
            var requestKey = request.Parameters.Where(p => p.Type == ParameterType.HttpHeader && p.Name == "RequestKey").FirstOrDefault();
            return requestKey?.Value;
        }

        private object GetRequestQueryStringAsObject(IRestRequest request)
        {
            var result = new Dictionary<string, string>();
            var parameters = request.Parameters.Where(p => p.Type == ParameterType.QueryString);
            var grouped = parameters.GroupBy(r => r.Name);

            foreach(var group in grouped)
            {
                result.Add(group.Key, string.Join(",", group.Select(r => r.Value)));
            }

            return result.Any() ? result : null;
        }

        private object GetRequestBody(IRestRequest request)
        {
            var body = request?.Parameters?.FirstOrDefault(p => p.Type == ParameterType.RequestBody);

            var isJson = request?.Parameters?.Exists(p => 
                (p.Type == ParameterType.HttpHeader &&
                p.Name == "Content-Type" && 
                p.Value?.ToString().Contains("json") == true)
                ||
                (p.Type == ParameterType.RequestBody &&
                p.Name?.ToString().Contains("json") == true))
                ?? false;

            var isForm = request?.Parameters?.Exists(p =>
                p.Type == ParameterType.HttpHeader &&
                p.Name == "Content-Type" &&
                p.Value?.ToString().Contains("x-www-form-urlencoded") == true) 
                ?? false;

            if (body != null && body.Value != null)
            {
                if (isJson)
                {
                    return this.GetContentAsObjectByContentTypeJson(body.Value.ToString(), true, this.Configuration.JsonBlacklist);
                }
                
                if (isForm)
                {
                    return this.GetContentAsObjectByContentTypeForm(body.Value.ToString());
                }
            }

            return body?.Value;
        }

        private object GetResponseContent(IRestResponse response)
        {
            var content = response.Content;

            bool isJson = response.ContentType?.Contains("json") == true;
                
            if (content != null && isJson == true)
            {
                return this.GetContentAsObjectByContentTypeJson(content, false, null);
            }

            return content;
        }

        private object GetContentAsObjectByContentTypeForm(string content)
        {
            var result = new Dictionary<string, string>();
            var parts = content.Split('&');
            var partsKeyValue = parts.Select(r =>
                new
                {
                    Key = HttpUtility.UrlDecode(r.Split('=').FirstOrDefault()),
                    Value = HttpUtility.UrlDecode(r.Split('=').Skip(1).LastOrDefault())
                });

            var grouped = partsKeyValue.GroupBy(r => r.Key);
            foreach (var group in grouped)
            {
                result.Add(group.Key, string.Join(",", group.Select(r => r.Value)));
            }

            return result;
        }

        private object GetContentAsObjectByContentTypeJson(string content, bool maskJson, string[] blacklist)
        {
            try
            {
                if (maskJson == true && blacklist?.Any() == true)
                {
                    content = content.MaskFields(blacklist, "******");
                }

                return content.DeserializeAsObject();
            }
            catch (Exception) { }

            return content;
        }

        private object GetRequestHeaders(IRestRequest request)
        {
            var result = new Dictionary<string, string>();
            var parameters = request.Parameters.Where(p => p.Type == ParameterType.HttpHeader);
            var grouped = parameters.GroupBy(r => r.Name);

            foreach (var group in grouped)
            {
                result.Add(group.Key, string.Join(",", group.Select(r => r.Value)));
            }

            return result;
        }

        private object GetResponseHeaders(IRestResponse response)
        {
            var result = new Dictionary<string, string>();
            var parameters = response?.Headers ?? new List<Parameter>();
            var grouped = parameters.GroupBy(r => r.Name);

            foreach (var group in grouped)
            {
                result.Add(group.Key, string.Join(",", group.Select(r => r.Value)));
            }

            return result.Any() ? result : null;
        }
    }
}