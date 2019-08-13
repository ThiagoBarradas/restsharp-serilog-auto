namespace RestSharp.Serilog.Auto
{
    public static class RestSharpExtension
    {
        public static string RequestKeyProperty { get; set; } = "RequestKey";

        public static string RequestKeyValue { get; set; }

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
    }
}
