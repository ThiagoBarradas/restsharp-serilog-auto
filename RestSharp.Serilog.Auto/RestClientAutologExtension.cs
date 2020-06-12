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

            restClientAutolog.AddDefaultParameter(key, value, ParameterType.HttpHeader);
            restClientAutolog.AdditionalProperties[key] = value;

            return restClientAutolog;
        }

        public static IRestClient EnableLog(this IRestClient restClient, bool enabled = true)
        {
            if (!(restClient is RestClientAutolog))
            {
                return restClient;
            }

            RestClientAutolog restClientAutolog = (RestClientAutolog)restClient;

            restClientAutolog.Configuration.EnabledLog = enabled;

            return restClientAutolog;
        }
    }
}
