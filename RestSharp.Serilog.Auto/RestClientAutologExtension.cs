namespace RestSharp.Serilog.Auto
{
    /// <summary>
    /// Defines the <see cref="RestClientAutologExtension" />
    /// </summary>
    public static class RestClientAutologExtension
    {
        /// <summary>
        /// The AddLogAdditionalInfo
        /// </summary>
        /// <param name="restClient">The restClient<see cref="IRestClient"/></param>
        /// <param name="key">The key<see cref="string"/></param>
        /// <param name="value">The value<see cref="string"/></param>
        /// <returns>The <see cref="IRestClient"/></returns>
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
