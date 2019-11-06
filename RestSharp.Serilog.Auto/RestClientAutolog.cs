namespace RestSharp
{
    using global::Serilog;
    using global::Serilog.Context;

    using JsonMasking;

    using PackUtils;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;

    /// <summary>
    /// Defines the <see cref="RestClientAutolog" />
    /// </summary>
    public class RestClientAutolog : RestClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestClientAutolog"/> class.
        /// </summary>
        public RestClientAutolog() => Startup(null);

        /// <summary>
        /// Initializes a new instance of the <see cref="RestClientAutolog"/> class.
        /// </summary>
        /// <param name="loggerConfiguration">The loggerConfiguration<see cref="LoggerConfiguration"/></param>
        public RestClientAutolog(LoggerConfiguration loggerConfiguration)
        {
            Startup(new RestClientAutologConfiguration { LoggerConfiguration = loggerConfiguration });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestClientAutolog"/> class.
        /// </summary>
        /// <param name="configuration">The configuration<see cref="RestClientAutologConfiguration"/></param>
        public RestClientAutolog(RestClientAutologConfiguration configuration)
        {
            Startup(configuration);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestClientAutolog"/> class.
        /// </summary>
        /// <param name="baseUrl">The baseUrl<see cref="string"/></param>
        public RestClientAutolog(string baseUrl)
            : base(baseUrl) => Startup(null);

        /// <summary>
        /// Initializes a new instance of the <see cref="RestClientAutolog"/> class.
        /// </summary>
        /// <param name="baseUrl">The baseUrl<see cref="string"/></param>
        /// <param name="loggerConfiguration">The loggerConfiguration<see cref="LoggerConfiguration"/></param>
        public RestClientAutolog(string baseUrl, LoggerConfiguration loggerConfiguration)
            : base(baseUrl) => Startup(new RestClientAutologConfiguration { LoggerConfiguration = loggerConfiguration });

        /// <summary>
        /// Initializes a new instance of the <see cref="RestClientAutolog"/> class.
        /// </summary>
        /// <param name="baseUrl">The baseUrl<see cref="string"/></param>
        /// <param name="configuration">The configuration<see cref="RestClientAutologConfiguration"/></param>
        public RestClientAutolog(string baseUrl, RestClientAutologConfiguration configuration)
            : base(baseUrl) => Startup(configuration);

        /// <summary>
        /// Initializes a new instance of the <see cref="RestClientAutolog"/> class.
        /// </summary>
        /// <param name="baseUrl">The baseUrl<see cref="string"/></param>
        /// <param name="message">The message<see cref="string"/></param>
        public RestClientAutolog(string baseUrl, string message)
            : base(baseUrl) => Startup(new RestClientAutologConfiguration { MessageTemplateForError = message, MessageTemplateForSuccess = message });

        /// <summary>
        /// Initializes a new instance of the <see cref="RestClientAutolog"/> class.
        /// </summary>
        /// <param name="baseUrl">The baseUrl<see cref="Uri"/></param>
        public RestClientAutolog(Uri baseUrl)
            : base(baseUrl) => Startup(null);

        /// <summary>
        /// Initializes a new instance of the <see cref="RestClientAutolog"/> class.
        /// </summary>
        /// <param name="baseUrl">The baseUrl<see cref="Uri"/></param>
        /// <param name="loggerConfiguration">The loggerConfiguration<see cref="LoggerConfiguration"/></param>
        public RestClientAutolog(Uri baseUrl, LoggerConfiguration loggerConfiguration)
            : base(baseUrl) => Startup(new RestClientAutologConfiguration { LoggerConfiguration = loggerConfiguration });

        /// <summary>
        /// Initializes a new instance of the <see cref="RestClientAutolog"/> class.
        /// </summary>
        /// <param name="baseUrl">The baseUrl<see cref="Uri"/></param>
        /// <param name="configuration">The configuration<see cref="RestClientAutologConfiguration"/></param>
        public RestClientAutolog(Uri baseUrl, RestClientAutologConfiguration configuration)
            : base(baseUrl) => Startup(configuration);

        /// <summary>
        /// Gets or sets the GlobalConfiguration
        /// </summary>
        public static RestClientAutologConfiguration GlobalConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the AdditionalProperties
        /// </summary>
        public Dictionary<string, string> AdditionalProperties { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets the Configuration
        /// </summary>
        public RestClientAutologConfiguration Configuration { get; set; }

        /// <summary>
        /// The Execute
        /// </summary>
        /// <param name="request">The request<see cref="IRestRequest"/></param>
        /// <returns>The <see cref="IRestResponse"/></returns>
        public override IRestResponse Execute(IRestRequest request)
        {
            var stopwatch = Stopwatch.StartNew();

            var response = base.Execute(request);
            stopwatch.Stop();

            LogRequestAndResponse(response, stopwatch);

            return response;
        }

        /// <summary>
        /// The ExecuteTaskAsync
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">The request<see cref="IRestRequest"/></param>
        /// <param name="token">The token<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task{IRestResponse{T}}"/></returns>
        public override async Task<IRestResponse<T>> ExecuteTaskAsync<T>(
            IRestRequest request,
            CancellationToken token)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await base.ExecuteTaskAsync<T>(request, token).ConfigureAwait(false);
            LogRequestAndResponse(response, stopwatch);

            return response;
        }

        /// <summary>
        /// The ExecuteTaskAsync
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request">The request<see cref="IRestRequest"/></param>
        /// <param name="token">The token<see cref="CancellationToken"/></param>
        /// <param name="httpMethod">The httpMethod<see cref="Method"/></param>
        /// <returns>The <see cref="Task{IRestResponse{T}}"/></returns>
        public override async Task<IRestResponse<T>> ExecuteTaskAsync<T>(
            IRestRequest request,
            CancellationToken token,
            Method httpMethod)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = await base.ExecuteTaskAsync<T>(request, token, httpMethod).ConfigureAwait(false);
            LogRequestAndResponse(response, stopwatch);

            return response;
        }

        /// <summary>
        /// The GetContentAsObjectByContentTypeForm
        /// </summary>
        /// <param name="content">The content<see cref="string"/></param>
        /// <returns>The <see cref="object"/></returns>
        private object GetContentAsObjectByContentTypeForm(string content) => content
            .Split('&')
            .Select(r => (Key: HttpUtility.UrlDecode(r.Split('=').FirstOrDefault()), Value: HttpUtility.UrlDecode(r.Split('=').Skip(1).LastOrDefault())))
            .GroupBy(r => r.Key)
            .ToDictionary(@group => group.Key, @group => string.Join(",", group.Select(r => r.Value)));

        /// <summary>
        /// The GetContentAsObjectByContentTypeJson
        /// </summary>
        /// <param name="content">The content<see cref="string"/></param>
        /// <param name="maskJson">The maskJson<see cref="bool"/></param>
        /// <param name="blacklist">The blacklist<see cref="string[]"/></param>
        /// <returns>The <see cref="object"/></returns>
        private object GetContentAsObjectByContentTypeJson(string content, bool maskJson, string[] blacklist)
        {
            try
            {
                if (maskJson && blacklist?.Any() == true)
                {
                    content = content.MaskFields(blacklist, "******");
                }

                return content.DeserializeAsObject();
            }
            catch (Exception)
            {
            }

            return content;
        }

        /// <summary>
        /// The GetIgnoredProperties
        /// </summary>
        /// <param name="request">The request<see cref="IRestRequest"/></param>
        /// <returns>The <see cref="string[]"/></returns>
        private string[] GetIgnoredProperties(IRestRequest request)
        {
            var ignoredProperties = request.Parameters?.Find(p => p.Type == ParameterType.HttpHeader && p.Name == "LogIgnored");

            return ignoredProperties?.Value == null ? (new string[] { }) : ignoredProperties.Value.ToString().Split(',');
        }

        /// <summary>
        /// The GetRequestBody
        /// </summary>
        /// <param name="request">The request<see cref="IRestRequest"/></param>
        /// <returns>The <see cref="object"/></returns>
        private object GetRequestBody(IRestRequest request)
        {
            var body = request?.Parameters?.FirstOrDefault(p => p.Type == ParameterType.RequestBody);

            bool isJson = request?.Parameters?.Exists(p => (p.Type == ParameterType.HttpHeader && p.Name == "Content-Type" && p.Value?.ToString().Contains("json") == true) || (p.Type == ParameterType.RequestBody && p.Name?.ToString().Contains("json") == true)) ?? false;

            bool isForm = request?.Parameters?.Exists(p => p.Type == ParameterType.HttpHeader && p.Name == "Content-Type" && p.Value?.ToString().Contains("x-www-form-urlencoded") == true) ?? false;

            if (body?.Value != null)
            {
                if (isJson)
                {
                    return GetContentAsObjectByContentTypeJson(body.Value.ToString(), true, Configuration.JsonBlacklist);
                }

                if (isForm)
                {
                    return GetContentAsObjectByContentTypeForm(body.Value.ToString());
                }
            }

            return body?.Value;
        }

        /// <summary>
        /// The GetRequestHeaders
        /// </summary>
        /// <param name="request">The request<see cref="IRestRequest"/></param>
        /// <returns>The <see cref="object"/></returns>
        private object GetRequestHeaders(IRestRequest request) => request.Parameters.Where(p => p.Type == ParameterType.HttpHeader).GroupBy(r => r.Name).ToDictionary(@group => @group.Key, @group => string.Join(",", @group.Select(r => r.Value)));

        /// <summary>
        /// The GetRequestQueryStringAsObject
        /// </summary>
        /// <param name="request">The request<see cref="IRestRequest"/></param>
        /// <returns>The <see cref="object"/></returns>
        private object GetRequestQueryStringAsObject(IRestRequest request)
        {
            var result = request.Parameters.Where(p => p.Type == ParameterType.QueryString).GroupBy(r => r.Name).ToDictionary(@group => group.Key, @group => string.Join(",", group.Select(r => r.Value)));

            return result.Count > 0 ? result : null;
        }

        /// <summary>
        /// The GetResponseContent
        /// </summary>
        /// <param name="response">The response<see cref="IRestResponse"/></param>
        /// <returns>The <see cref="object"/></returns>
        private object GetResponseContent(IRestResponse response)
        {
            string content = response.Content;

            bool isJson = response.ContentType?.Contains("json") == true;

            if (content != null && isJson)
            {
                return GetContentAsObjectByContentTypeJson(content, false, null);
            }

            return content;
        }

        /// <summary>
        /// The GetResponseHeaders
        /// </summary>
        /// <param name="response">The response<see cref="IRestResponse"/></param>
        /// <returns>The <see cref="object"/></returns>
        private object GetResponseHeaders(IRestResponse response)
        {
            var result = (response?.Headers ?? new List<Parameter>()).GroupBy(r => r.Name).ToDictionary(@group => group.Key, @group => string.Join(",", group.Select(r => r.Value)));

            return result.Count > 0 ? result : null;
        }

        /// <summary>
        /// The LogRequestAndResponse
        /// </summary>
        /// <param name="response">The response<see cref="IRestResponse"/></param>
        /// <param name="stopwatch">The stopwatch<see cref="Stopwatch"/></param>
        private void LogRequestAndResponse(IRestResponse response, Stopwatch stopwatch)
        {
            if (Configuration.LoggerConfiguration != null)
            {
                Log.Logger = Configuration.LoggerConfiguration.CreateLogger();
            }

            var uri = BuildUri(response.Request);
            var ignoredProperties = GetIgnoredProperties(response.Request);
            var properties = new Dictionary<string, object>();

            if (AdditionalProperties?.Any() == true)
            {
                foreach (KeyValuePair<string, string> item in AdditionalProperties)
                {
                    properties.Add(item.Key, item.Value);
                }
            }

            properties.Add("Agent", "RestSharp");
            properties.Add("ElapsedMilliseconds", stopwatch.ElapsedMilliseconds);
            properties.Add("Method", response.Request.Method.ToString());
            properties.Add("Url", uri.AbsoluteUri);
            properties.Add("Host", uri.Host);
            properties.Add("Path", uri.AbsolutePath);
            properties.Add("Port", uri.Port);
            properties.Add("QueryString", uri.Query);
            properties.Add("Query", GetRequestQueryStringAsObject(response.Request));
            properties.Add("RequestBody", GetRequestBody(response.Request));
            properties.Add("RequestHeaders", GetRequestHeaders(response.Request));
            properties.Add("StatusCode", (int)response.StatusCode);
            properties.Add("StatusCodeFamily", ((int)response.StatusCode).ToString()[0] + "XX");
            properties.Add("StatusDescription", response.StatusDescription?.Replace(" ", string.Empty));
            properties.Add("ResponseStatus", response.ResponseStatus.ToString());
            properties.Add("ProtocolVersion", response.ProtocolVersion);
            properties.Add("IsSuccessful", response.IsSuccessful);
            properties.Add("ErrorMessage", response.ErrorMessage);
            properties.Add("ErrorException", response.ErrorException);
            properties.Add("ResponseContent", GetResponseContent(response));
            properties.Add("ContentLength", response.ContentLength);
            properties.Add("ContentType", response.ContentType);
            properties.Add("ResponseHeaders", GetResponseHeaders(response));
            properties.Add("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));

            foreach (var property in properties.Where(property => !ignoredProperties.Contains(property.Key)))
            {
                LogContext.PushProperty(property.Key, property.Value);
            }

            if (response.IsSuccessful)
            {
                Log.Information(Configuration.MessageTemplateForSuccess);
            }
            else
            {
                Log.Error(Configuration.MessageTemplateForSuccess);
            }
        }

        /// <summary>
        /// The Startup
        /// </summary>
        /// <param name="configuration">The configuration<see cref="RestClientAutologConfiguration"/></param>
        private void Startup(RestClientAutologConfiguration configuration)
        {
            if (configuration == null)
            {
                configuration = GlobalConfiguration != null ? GlobalConfiguration.Clone() : new RestClientAutologConfiguration();
            }

            Configuration = configuration;
        }
    }
}
