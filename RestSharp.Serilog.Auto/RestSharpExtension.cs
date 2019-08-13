namespace RestSharp.Serilog.Auto
{
    public static class RestSharpExtension
    {
        public static string RequestKeyProperty { get; set; } = "RequestKey";

        public static string AccountIdProperty { get; set; } = "AccountId";

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

        public static IRestClient WithHeaders(this IRestClient client, string requestKey, string accountId)
        {
            return client.WithRequestKey(requestKey).WithAccountId(accountId);
        }

        public static IRestRequest WithHeaders(this IRestRequest request, string requestKey, string accountId)
        {
            return request.WithRequestKey(requestKey).WithAccountId(accountId);
        }
    }
}
