using JsonMasking;
using Newtonsoft.Json;
using RestSharp.Serilog.Auto.Extensions;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace RestSharp
{
    public class RestClientAutolog : RestClient, IRestClient
    {
        public Dictionary<string, string> AdditionalProperties { get; set; } = new Dictionary<string, string>();

        public static RestClientAutologConfiguration GlobalConfiguration { get; set; }

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
            this.Startup(null);
        }

        public RestClientAutolog(Uri baseUrl) : base(baseUrl)
        {
            this.Startup(null);
        }

        public RestClientAutolog()
        {
            this.Startup(null);
        }

        private void Startup(RestClientAutologConfiguration configuration)
        {
            if (configuration == null)
            { 
                configuration = (GlobalConfiguration != null)
                    ? GlobalConfiguration.Clone()
                    : new RestClientAutologConfiguration();
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

        public new Task<IRestResponse> ExecuteAsync(IRestRequest request, Method method, CancellationToken token = default)
        {
            request.Method = method;
            return this.ExecuteAsync(request, token);
        }

        public new async Task<IRestResponse> ExecuteAsync(IRestRequest request, CancellationToken token = default)
        {
            var stopwatch = Stopwatch.StartNew();

            var response = await base.ExecuteAsync(request, token);
            stopwatch.Stop();

            this.LogRequestAndResponse(response, stopwatch);

            return response;
        }

        public new async Task<IRestResponse<T>> ExecuteAsync<T>(IRestRequest request, CancellationToken token = default)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await base.ExecuteAsync<T>(request, token);
            stopwatch.Stop();

            this.LogRequestAndResponse(response, stopwatch);

            return response;
        }

        private void LogRequestAndResponse(IRestResponse response, Stopwatch stopwatch)
        {
            if (!this.Configuration.EnabledLog)
            {
                return;
            }

            if (this.Configuration.LoggerConfiguration != null)
            {
                Log.Logger = this.Configuration.LoggerConfiguration.CreateLogger();
            }

            var uri = this.BuildUri(response.Request);
            string[] ignoredProperties = this.GetIgnoredProperties(response.Request);
            var properties = new Dictionary<string, object>();

            if (this.AdditionalProperties?.Any() == true)
            {
                foreach(var item in this.AdditionalProperties)
                {
                    properties.Add(item.Key, item.Value);
                }
            }

            string exceptionMessage = null;
            string exceptionStackTrace = null;

            if (response.ErrorMessage != null)
            {
                exceptionMessage = HandleFieldSize(response.ErrorMessage, ExceptionMaxLenghtExtension.ErrorMessageLenght);
            }
            
            if (response.ErrorException != null)
            {
                exceptionStackTrace = HandleFieldSize(response.ErrorException.StackTrace, ExceptionMaxLenghtExtension.ErrorExceptionLenght);
            }

            properties.Add("Agent", "RestSharp");
            properties.Add("ElapsedMilliseconds", stopwatch.ElapsedMilliseconds);
            properties.Add("Method", response.Request.Method.ToString());
            properties.Add("Url", BuildUrl(uri));
            properties.Add("Host", uri.Host);
            properties.Add("Path", uri.AbsolutePath);
            properties.Add("Port", uri.Port);
            properties.Add("QueryString", MaskUriQueryString(uri));
            properties.Add("Query", this.GetRequestQueryStringAsObject(response.Request, true));
            properties.Add("RequestBody", this.GetRequestBody(response.Request));
            properties.Add("RequestHeaders", this.GetRequestHeaders(response.Request, true));
            properties.Add("StatusCode", (int)response.StatusCode);
            properties.Add("StatusCodeFamily", ((int)response.StatusCode).ToString()[0] + "XX");
            properties.Add("StatusDescription", response.StatusDescription?.Replace(" ", ""));
            properties.Add("ResponseStatus", response.ResponseStatus.ToString());
            properties.Add("IsSuccessful", response.IsSuccessful);
            properties.Add("ErrorMessage", exceptionMessage);
            properties.Add("ErrorException", exceptionStackTrace);
            properties.Add("ResponseContent", this.GetResponseContent(response));
            properties.Add("ContentLength", response.ContentLength);
            properties.Add("ContentType", response.ContentType);
            properties.Add("ResponseHeaders", this.GetResponseHeaders(response, true));
            properties.Add("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
            properties.Add("ProtocolVersion", response.ProtocolVersion?.ToString());

            foreach (var property in properties)
            {
                if (ignoredProperties.Contains(property.Key) == false)
                {
                    LogContext.PushProperty(property.Key, property.Value);
                }
            }

            LogEventLevel level = (response.IsSuccessful) ? LogEventLevel.Information : LogEventLevel.Error;

            if (this.Configuration.OverrideLogLevelByStatusCode?.Any(r => r.Key == response.StatusCode) == true)
            {
                level = this.Configuration.OverrideLogLevelByStatusCode.First(r => r.Key == response.StatusCode).Value;
            }

            var message = (level == LogEventLevel.Warning ||
                           level == LogEventLevel.Error ||
                           level == LogEventLevel.Fatal)
                           ? this.Configuration.MessageTemplateForError
                           : this.Configuration.MessageTemplateForSuccess;

            Log.Logger.Write(level, message);
        }

        private string[] GetIgnoredProperties(IRestRequest request)
        {
            var ignoredProperties = request.Parameters.Where(p => 
                p.Type == ParameterType.HttpHeader && p.Name == "LogIgnored")
                .FirstOrDefault();

            if (ignoredProperties?.Value == null)
            {
                return new string[] { };
            }

            return ignoredProperties.Value.ToString().Split(',');
        }

        private object GetRequestQueryStringAsObject(IRestRequest request, bool mask)
        {
            var result = new Dictionary<string, string>();
            var parameters = request.Parameters.Where(p => p.Type == ParameterType.QueryString);
            var grouped = parameters.GroupBy(r => r.Name);

            foreach(var group in grouped)
            {
                var key = group.Key;
                var value = string.Join(",", group.Select(r => r.Value));

                if (mask)
                {
                    value = MaskQueryStringValue(key, value);
                }

                result.Add(group.Key, value);
            }

            return result.Any() ? result : null;
        }

        private object GetRequestBody(IRestRequest request)
        {
            var body = request?.Parameters?.FirstOrDefault(p => p.Type == ParameterType.RequestBody);

            var isJson = request?.Parameters?.Exists(p => 
                (p.Type == ParameterType.HttpHeader &&
                (p.Name == "Content-Type") && 
                p.Value?.ToString().Contains("json") == true)
                ||
                (p.Type == ParameterType.RequestBody &&
                (p.Name?.ToString().Contains("json") == true ||
                 p.DataFormat == DataFormat.Json ||
                 p.ContentType?.Contains("application/json") == true)))
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
                    var content = (body.Value is string) ? body.Value.ToString() : JsonConvert.SerializeObject(body.Value);
                    return this.GetContentAsObjectByContentTypeJson(content, true, this.Configuration.RequestJsonBlacklist);
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
                return this.GetContentAsObjectByContentTypeJson(content, true, this.Configuration.ResponseJsonBlacklist);
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

        private object GetRequestHeaders(IRestRequest request, bool mask)
        {
            var result = new Dictionary<string, string>();
            var requestParameters = request.Parameters.Where(p => p.Type == ParameterType.HttpHeader);
            var clientParameters = this.DefaultParameters.Where(p => p.Type == ParameterType.HttpHeader);

            var parameters = requestParameters.Union(clientParameters);

            var grouped = parameters.GroupBy(r => r.Name);

            foreach (var group in grouped)
            {
                var key = group.Key;
                var value = string.Join(",", group.Select(r => r.Value));

                if (mask)
                {
                    value = MaskHeaderValue(key, value);
                }

                result.Add(group.Key, value);
            }

            return result;
        }

        private object GetResponseHeaders(IRestResponse response, bool mask)
        {
            var result = new Dictionary<string, string>();
            var parameters = response?.Headers ?? new List<Parameter>();
            var grouped = parameters.GroupBy(r => r.Name);

            foreach (var group in grouped)
            {
                var key = group.Key;
                var value = string.Join(",", group.Select(r => r.Value));

                if (mask)
                {
                    value = MaskHeaderValue(key, value);
                }

                result.Add(group.Key, value);
            }

            return result.Any() ? result : null;
        }

        private static string HandleFieldSize(string value, int maxSize, bool required = false, string defaultValue = "????")
        {
            if (string.IsNullOrWhiteSpace(value) && !required)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                value = defaultValue;
            }

            if (value.Length > maxSize)
            {
                return value.Substring(0, maxSize);
            }

            return value;
        }

        private string MaskUriQueryString(Uri uri)
        {
            if (string.IsNullOrEmpty(uri.Query))
            {
                return string.Empty;
            }

            var queryString = HttpUtility.ParseQueryString(uri.Query);
            foreach (var qs in queryString.AllKeys)
            {
                queryString[qs] = MaskQueryStringValue(qs, queryString[qs]);
            }
            
            return queryString.ToString();
        }

        private string MaskQueryStringValue(string key, string value)
        {
            if (this.Configuration.QueryStringBlacklist?.Any() == true && this.Configuration.QueryStringBlacklist.Contains(key))
            {
                return "******";
            }

            return value;
        }

        private string MaskHeaderValue(string key, string value)
        {
            if (this.Configuration.HeaderBlacklist?.Any() == true && this.Configuration.HeaderBlacklist.Contains(key))
            {
                return "******";
            }

            return value;
        }

        private string BuildUrl(Uri uri)
        {
            var url = uri.AbsoluteUri;

            if(uri.Query != null)
            {
                url = $"{url.Split('?')?.FirstOrDefault()}?{MaskUriQueryString(uri)}";
            }

            return url;
        }
    }
}