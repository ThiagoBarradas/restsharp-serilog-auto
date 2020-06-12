using System;

namespace RestSharp.Serilog.Auto.Extensions
{
    public static class ExceptionMaxLenghtExtension
    {
        public static int ErrorMessageLenght = 256;

        public static int ErrorExceptionLenght = 1024;

        static ExceptionMaxLenghtExtension()
        {
            var errorMessageMaxLength = Environment.GetEnvironmentVariable("SERILOG_ERROR_MESSAGE_MAX_LENGTH");
            if (!string.IsNullOrWhiteSpace(errorMessageMaxLength))
            {
                int.TryParse(errorMessageMaxLength, out ErrorMessageLenght);
            }

            var errorExceptionMaxLength = Environment.GetEnvironmentVariable("SERILOG_ERROR_EXCEPTION_MAX_LENGTH");
            if (!string.IsNullOrWhiteSpace(errorExceptionMaxLength))
            {
                int.TryParse(errorExceptionMaxLength, out ErrorExceptionLenght);
            }
        }
    }
}
