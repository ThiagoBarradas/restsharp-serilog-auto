namespace RestSharp.Serilog.Auto
{
    public static class RestSharpExtension
    {
        public static string RequestKeyProperty { get; set; } = "RequestKey";

        public static string RequestKeyValue { get; set; }

        public static string AccountIdProperty { get; set; } = "AccountId";

        public static string AccountIdValue { get; set; }

        public static IRestClient WithRequestKey(this IRestClient client)
        {
            client.AddDefaultHeader(RequestKeyProperty, RequestKeyValue ?? "undefined");
            return client;
        }

        public static IRestRequest WithRequestKey(this IRestRequest request)
        {
            request.AddHeader(RequestKeyProperty, RequestKeyValue ?? "undefined");
            return request;
        }

        public static IRestClient WithAccountId(this IRestClient client)
        {
            client.AddDefaultHeader(AccountIdProperty, AccountIdValue ?? "undefined");
            return client;
        }

        public static IRestRequest WithAccountId(this IRestRequest request)
        {
            request.AddHeader(AccountIdProperty, AccountIdValue ?? "undefined");
            return request;
        }

        public static IRestClient WithHeaders(this IRestClient client)
        {
            return client.WithRequestKey().WithAccountId();
        }
    }
}
