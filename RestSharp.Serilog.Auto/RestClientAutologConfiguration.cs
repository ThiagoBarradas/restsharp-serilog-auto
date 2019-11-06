using Serilog;

namespace RestSharp
{
    /// <summary>
    /// Defines the <see cref="RestClientAutologConfiguration" />
    /// </summary>
    public class RestClientAutologConfiguration
    {
        /// <summary>
        /// Defines the DefaultMessageTemplateForError
        /// </summary>
        internal const string DefaultMessageTemplateForError =
            "[{Application}] HTTP {Method} {Url} responded {StatusCode} in {ElapsedMilliseconds} ms";

        /// <summary>
        /// Defines the DefaultMessageTemplateForSuccess
        /// </summary>
        internal const string DefaultMessageTemplateForSuccess =
            "[{Application}] HTTP {Method} {Url} responded {StatusCode} in {ElapsedMilliseconds} ms";

        /// <summary>
        /// Defines the _messageTemplateForError
        /// </summary>
        private string _messageTemplateForError;

        /// <summary>
        /// Defines the _messageTemplateForSuccess
        /// </summary>
        private string _messageTemplateForSuccess;

        /// <summary>
        /// Gets or sets the JsonBlacklist
        /// </summary>
        public string[] JsonBlacklist { get; set; }

        /// <summary>
        /// Gets or sets the LoggerConfiguration
        /// </summary>
        public LoggerConfiguration LoggerConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the MessageTemplateForError
        /// </summary>
        public string MessageTemplateForError
        {
            get => string.IsNullOrWhiteSpace(_messageTemplateForError) ? DefaultMessageTemplateForError : _messageTemplateForError;
            set => _messageTemplateForError = value;
        }

        /// <summary>
        /// Gets or sets the MessageTemplateForSuccess
        /// </summary>
        public string MessageTemplateForSuccess
        {
            get => string.IsNullOrWhiteSpace(_messageTemplateForSuccess) ? DefaultMessageTemplateForSuccess : _messageTemplateForSuccess;
            set => _messageTemplateForSuccess = value;
        }

        /// <summary>
        /// The Clone
        /// </summary>
        /// <returns>The <see cref="RestClientAutologConfiguration"/></returns>
        public RestClientAutologConfiguration Clone() => (RestClientAutologConfiguration)MemberwiseClone();
    }
}
