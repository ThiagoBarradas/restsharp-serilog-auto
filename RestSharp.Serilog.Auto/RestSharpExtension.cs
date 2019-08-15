namespace RestSharp.Serilog.Auto
{
  public static class RestSharpExtension
  {
    #region Public Properties

    public static string AccountIdProperty { get; set; } = "AccountId";
    public static string RequestKeyProperty { get; set; } = "RequestKey";

    #endregion

    #region Public Methods and Operators

    public static IRestClient WithAccountId(this IRestClient client, string accountId)
    {
      client.AddDefaultHeader(AccountIdProperty, accountId ?? "undefined");
      return client;
    }

    public static IRestRequest WithAccountId(this IRestRequest request, string accountId)
    {
      request.AddHeader(AccountIdProperty, accountId ?? "undefined");
      return request;
    }

    public static IRestClient WithHeaders(this IRestClient client, string requestKey, string accountId) =>
      client.WithRequestKey(requestKey).WithAccountId(accountId);

    public static IRestRequest WithHeaders(this IRestRequest request, string requestKey, string accountId) =>
      request.WithRequestKey(requestKey).WithAccountId(accountId);

    public static IRestClient WithRequestKey(this IRestClient client, string requestKey)
    {
      client.AddDefaultHeader(RequestKeyProperty, requestKey ?? "undefined");
      return client;
    }

    public static IRestRequest WithRequestKey(this IRestRequest request, string requestKey)
    {
      request.AddHeader(RequestKeyProperty, requestKey ?? "undefined");
      return request;
    }

    #endregion
  }
}