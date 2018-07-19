using Serilog;

namespace RestSharp
{
    public class RestClientAutologConfiguration
    {
        internal const string DefaultMessageTemplateForError =
            "[{Application}] HTTP {Method} {Uri} responded {StatusCode} in {ElapsedMilliseconds} ms";

        internal const string DefaultMessageTemplateForSuccess =
            "[{Application}] HTTP {Method} {Uri} responded {StatusCode} in {ElapsedMilliseconds} ms";

        private string _messageTemplateForError;
        public string MessageTemplateForError
        {
            get
            {
                return (string.IsNullOrWhiteSpace(this._messageTemplateForError))
                    ? RestClientAutologConfiguration.DefaultMessageTemplateForError
                    : this._messageTemplateForError;
            }
            set
            {
                this._messageTemplateForError = value;
            }
        }

        private string _messageTemplateForSuccess;
        public string MessageTemplateForSuccess
        {
            get
            {
                return (string.IsNullOrWhiteSpace(this._messageTemplateForSuccess))
                    ? RestClientAutologConfiguration.DefaultMessageTemplateForSuccess
                    : this._messageTemplateForSuccess;
            }
            set
            {
                this._messageTemplateForSuccess = value;
            }
        }

        private LoggerConfiguration _loggerConfiguration;
        public LoggerConfiguration LoggerConfiguration
        {
            get
            {
                return (this._loggerConfiguration != null)
                    ? this._loggerConfiguration
                    : new LoggerConfiguration()
                            .MinimumLevel.Debug()
                            .Enrich.FromLogContext()
                            .Enrich.WithProperty("Application", "RestSharp.Autolog")
                            .WriteTo.Console();
            }
            set
            {
                if (value != null)
                {
                    this._loggerConfiguration = value;
                }
            }
        }
    }
}
