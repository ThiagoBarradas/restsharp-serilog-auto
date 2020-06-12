using System;

namespace RestSharp.Serilog.Auto
{
    public static class RestClientAutologExtension
    {
        static RestClientAutologExtension()
        {
            var errorMessageMaxLength = Environment.GetEnvironmentVariable("SERILOG_ERROR_MESSAGE_MAX_LENGTH");
            if (!string.IsNullOrWhiteSpace(errorMessageMaxLength))
            {
                int.TryParse(errorMessageMaxLength, out RestClientAutolog.ErrorMessageLenght);
            }
            
            var errorExceptionMaxLength = Environment.GetEnvironmentVariable("SERILOG_ERROR_EXCEPTION_MAX_LENGTH");
            if (!string.IsNullOrWhiteSpace(errorExceptionMaxLength))
            {
                int.TryParse(errorExceptionMaxLength, out RestClientAutolog.ErrorExceptionLenght);
            }
        }

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
