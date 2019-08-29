namespace RestSharp.Serilog.Auto
{
    public static class RestClientAutologExtension
    {
        public static IRestClient AddLogAdditionalInfo(this IRestClient restClient, string key, string value)
        {
            if (!(restClient is RestClientAutolog))
            {
                return restClient;
            }

            RestClientAutolog restClientAutolog = (RestClientAutolog)restClient;

            restClientAutolog.AddDefaultHeader(key, value);
            restClientAutolog.AdditionalProperties[key] = value;

            return restClientAutolog;
        }
    }
}
