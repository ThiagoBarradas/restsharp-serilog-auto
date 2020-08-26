using Serilog;
using Serilog.Events;
using System.Collections.Generic;
using System.Net;

namespace RestSharp
{
    public class RestClientAutologConfiguration
    {
        internal const string DefaultMessageTemplateForError =
            "[{Application}] HTTP {Method} {Url} responded {StatusCode} in {ElapsedMilliseconds} ms";

        internal const string DefaultMessageTemplateForSuccess =
            "[{Application}] HTTP {Method} {Url} responded {StatusCode} in {ElapsedMilliseconds} ms";

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

        public string[] JsonBlacklist { get; set; }

        public bool EnabledLog { get; set; } = true;

        public Dictionary<HttpStatusCode, LogEventLevel> OverrideLogLevelByStatusCode { get; set; }

        public LoggerConfiguration LoggerConfiguration { get; set; }

        public RestClientAutologConfiguration Clone()
        {
            return (RestClientAutologConfiguration)this.MemberwiseClone();
        }
    }
}
