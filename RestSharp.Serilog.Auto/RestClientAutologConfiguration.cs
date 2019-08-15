namespace RestSharp
{
  using global::Serilog;

  public class RestClientAutologConfiguration
  {
    #region Constants

    internal const string DefaultMessageTemplateForError =
      "[{Application}] HTTP {Method} {Url} responded {StatusCode} in {ElapsedMilliseconds} ms";

    internal const string DefaultMessageTemplateForSuccess =
      "[{Application}] HTTP {Method} {Url} responded {StatusCode} in {ElapsedMilliseconds} ms";

    #endregion

    #region Fields

    private string _messageTemplateForError;

    private string _messageTemplateForSuccess;

    #endregion

    #region Public Properties

    public string[] JsonBlacklist { get; set; }

    public LoggerConfiguration LoggerConfiguration { get; set; }

    public string MessageTemplateForError
    {
      get =>
        string.IsNullOrWhiteSpace(this._messageTemplateForError)
          ? DefaultMessageTemplateForError
          : this._messageTemplateForError;
      set => this._messageTemplateForError = value;
    }

    public string MessageTemplateForSuccess
    {
      get =>
        string.IsNullOrWhiteSpace(this._messageTemplateForSuccess)
          ? DefaultMessageTemplateForSuccess
          : this._messageTemplateForSuccess;
      set => this._messageTemplateForSuccess = value;
    }

    #endregion

    #region Public Methods and Operators

    public RestClientAutologConfiguration Clone() => (RestClientAutologConfiguration) MemberwiseClone();

    #endregion
  }
}