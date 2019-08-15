namespace RestSharp
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using System.Web;
  using global::Serilog;
  using global::Serilog.Context;
  using JsonMasking;
  using PackUtils;

  public class RestClientAutolog : RestClient
  {
    #region Constructors and Destructors

    public RestClientAutolog(RestClientAutologConfiguration configuration)
    {
      Startup(configuration);
    }

    public RestClientAutolog(Uri baseUrl, RestClientAutologConfiguration configuration) : base(baseUrl)
    {
      Startup(configuration);
    }

    public RestClientAutolog(string baseUrl, RestClientAutologConfiguration configuration) : base(baseUrl)
    {
      Startup(configuration);
    }

    public RestClientAutolog(LoggerConfiguration loggerConfiguration)
    {
      Startup(new RestClientAutologConfiguration
      {
        LoggerConfiguration = loggerConfiguration
      });
    }

    public RestClientAutolog(Uri baseUrl, LoggerConfiguration loggerConfiguration) : base(baseUrl)
    {
      Startup(new RestClientAutologConfiguration
      {
        LoggerConfiguration = loggerConfiguration
      });
    }

    public RestClientAutolog(string baseUrl, LoggerConfiguration loggerConfiguration) : base(baseUrl)
    {
      Startup(new RestClientAutologConfiguration
      {
        LoggerConfiguration = loggerConfiguration
      });
    }

    public RestClientAutolog(string baseUrl, string message) : base(baseUrl)
    {
      Startup(new RestClientAutologConfiguration
      {
        MessageTemplateForError = message,
        MessageTemplateForSuccess = message
      });
    }

    public RestClientAutolog(string baseUrl) : base(baseUrl)
    {
      Startup(null);
    }

    public RestClientAutolog(Uri baseUrl) : base(baseUrl)
    {
      Startup(null);
    }

    public RestClientAutolog()
    {
      Startup(null);
    }

    #endregion

    #region Public Properties

    public static RestClientAutologConfiguration GlobalConfiguration { get; set; }

    public RestClientAutologConfiguration Configuration { get; set; }

    #endregion

    #region Public Methods and Operators

    public override IRestResponse Execute(IRestRequest request)
    {
      var stopwatch = Stopwatch.StartNew();

      var response = base.Execute(request);
      stopwatch.Stop();

      LogRequestAndResponse(response, stopwatch);

      return response;
    }

    public override async Task<IRestResponse<T>> ExecuteTaskAsync<T>(
      IRestRequest request,
      CancellationToken token)
    {
      var stopwatch = Stopwatch.StartNew();
      var response = await base.ExecuteTaskAsync<T>(request, token).ConfigureAwait(false);
      LogRequestAndResponse(response, stopwatch);

      return response;
    }

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

    #endregion

    #region Methods

    private object GetAccountId(IRestRequest request)
    {
      var accountId = request.Parameters.Where(p => p.Type == ParameterType.HttpHeader && p.Name == "AccountId")
        .FirstOrDefault();
      return accountId?.Value;
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
      foreach (var group in grouped) result.Add(group.Key, string.Join(",", group.Select(r => r.Value)));

      return result;
    }

    private object GetContentAsObjectByContentTypeJson(string content, bool maskJson, string[] blacklist)
    {
      try
      {
        if (maskJson && blacklist?.Any() == true) content = content.MaskFields(blacklist, "******");

        return content.DeserializeAsObject();
      }
      catch (Exception)
      {
      }

      return content;
    }

    private string[] GetIgnoredProperties(IRestRequest request)
    {
      var ignoredProperties = request.Parameters.Where(p =>
          p.Type == ParameterType.HttpHeader && p.Name == "LogIgnored")
        .FirstOrDefault();

      if (ignoredProperties?.Value == null) return new string[] { };

      return ignoredProperties.Value.ToString().Split(',');
    }

    private object GetRequestBody(IRestRequest request)
    {
      var body = request?.Parameters?.FirstOrDefault(p => p.Type == ParameterType.RequestBody);

      var isJson = request?.Parameters?.Exists(p =>
                     p.Type == ParameterType.HttpHeader &&
                     p.Name == "Content-Type" &&
                     p.Value?.ToString().Contains("json") == true
                     ||
                     p.Type == ParameterType.RequestBody &&
                     p.Name?.ToString().Contains("json") == true)
                   ?? false;

      var isForm = request?.Parameters?.Exists(p =>
                     p.Type == ParameterType.HttpHeader &&
                     p.Name == "Content-Type" &&
                     p.Value?.ToString().Contains("x-www-form-urlencoded") == true)
                   ?? false;

      if (body != null && body.Value != null)
      {
        if (isJson)
          return GetContentAsObjectByContentTypeJson(body.Value.ToString(), true, Configuration.JsonBlacklist);

        if (isForm) return GetContentAsObjectByContentTypeForm(body.Value.ToString());
      }

      return body?.Value;
    }

    private object GetRequestHeaders(IRestRequest request)
    {
      var result = new Dictionary<string, string>();
      var parameters = request.Parameters.Where(p => p.Type == ParameterType.HttpHeader);
      var grouped = parameters.GroupBy(r => r.Name);

      foreach (var group in grouped) result.Add(group.Key, string.Join(",", group.Select(r => r.Value)));

      return result;
    }

    private object GetRequestKey(IRestRequest request)
    {
      var requestKey = request.Parameters.Where(p => p.Type == ParameterType.HttpHeader && p.Name == "RequestKey")
        .FirstOrDefault();
      return requestKey?.Value;
    }

    private object GetRequestQueryStringAsObject(IRestRequest request)
    {
      var result = new Dictionary<string, string>();
      var parameters = request.Parameters.Where(p => p.Type == ParameterType.QueryString);
      var grouped = parameters.GroupBy(r => r.Name);

      foreach (var group in grouped) result.Add(group.Key, string.Join(",", group.Select(r => r.Value)));

      return result.Any() ? result : null;
    }

    private object GetResponseContent(IRestResponse response)
    {
      var content = response.Content;

      bool isJson = response.ContentType?.Contains("json") == true;

      if (content != null && isJson) return GetContentAsObjectByContentTypeJson(content, false, null);

      return content;
    }

    private object GetResponseHeaders(IRestResponse response)
    {
      var result = new Dictionary<string, string>();
      var parameters = response?.Headers ?? new List<Parameter>();
      var grouped = parameters.GroupBy(r => r.Name);

      foreach (var group in grouped) result.Add(group.Key, string.Join(",", group.Select(r => r.Value)));

      return result.Any() ? result : null;
    }

    private void LogRequestAndResponse(IRestResponse response, Stopwatch stopwatch)
    {
      if (Configuration.LoggerConfiguration != null) Log.Logger = Configuration.LoggerConfiguration.CreateLogger();

      var uri = BuildUri(response.Request);
      string[] ignoredProperties = GetIgnoredProperties(response.Request);
      var properties = new Dictionary<string, object>
      {
        {"Agent", "RestSharp"},
        {"ElapsedMilliseconds", stopwatch.ElapsedMilliseconds},
        {"Method", response.Request.Method.ToString()},
        {"Url", uri.AbsoluteUri},
        {"Host", uri.Host},
        {"Path", uri.AbsolutePath},
        {"Port", uri.Port},
        {"RequestKey", GetRequestKey(response.Request)},
        {"AccountId", GetAccountId(response.Request)},
        {"QueryString", uri.Query},
        {"Query", GetRequestQueryStringAsObject(response.Request)},
        {"RequestBody", GetRequestBody(response.Request)},
        {"RequestHeaders", GetRequestHeaders(response.Request)},
        {"StatusCode", (int) response.StatusCode},
        {"StatusCodeFamily", ((int) response.StatusCode).ToString()[0] + "XX"},
        {"StatusDescription", response.StatusDescription?.Replace(" ", "")},
        {"ResponseStatus", response.ResponseStatus.ToString()},
        {"ProtocolVersion", response.ProtocolVersion},
        {"IsSuccessful", response.IsSuccessful},
        {"ErrorMessage", response.ErrorMessage},
        {"ErrorException", response.ErrorException},
        {"ResponseContent", GetResponseContent(response)},
        {"ContentLength", response.ContentLength},
        {"ContentType", response.ContentType},
        {"ResponseHeaders", GetResponseHeaders(response)}
      };

      foreach (var property in properties)
      {
        if (ignoredProperties.Contains(property.Key) == false)
          LogContext.PushProperty(property.Key, property.Value);
      }

      if (response.IsSuccessful)
        Log.Information(Configuration.MessageTemplateForSuccess);
      else
        Log.Error(Configuration.MessageTemplateForSuccess);
    }

    private void Startup(RestClientAutologConfiguration configuration)
    {
      if (configuration == null)
      {
        configuration = GlobalConfiguration != null
          ? GlobalConfiguration.Clone()
          : new RestClientAutologConfiguration();
      }

      Configuration = configuration;
    }

    #endregion
  }
}