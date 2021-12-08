using Serilog;
using Serilog.Events;

using System;
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

        /// <summary>
        /// This property will redirect to the property 'RequestJsonBlacklist'. So, when you get/set it's value,
        /// what you are actually doing is using the other one.
        /// You should actually use the 'RequestJsonBlacklist' to dismiss the obsolete warning.
        /// </summary>
        [Obsolete("Keeping this property to retrocompatibility and to not break the code when updating the library.")]
        public string[] JsonBlacklist 
        {
            get => RequestJsonBlacklist;
            set => RequestJsonBlacklist = value;
        }

        public string[] RequestJsonBlacklist { get; set; }

        public string[] ResponseJsonBlacklist { get; set; }
        
        public string[] HeaderBlacklist { get; set; }

        public string[] QueryStringBlacklist { get; set; }

        public bool EnabledLog { get; set; } = true;

        public Dictionary<HttpStatusCode, LogEventLevel> OverrideLogLevelByStatusCode { get; set; }

        public LoggerConfiguration LoggerConfiguration { get; set; }

        public RestClientAutologConfiguration Clone()
        {
            return (RestClientAutologConfiguration)this.MemberwiseClone();
        }
    }
}
