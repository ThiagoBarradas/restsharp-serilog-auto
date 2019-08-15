namespace RestSharp.Serilog.Auto
{
  using System;

  public interface IRestClientFactory
  {
    #region Public Properties

    string AccountId { get; set; }
    string RequestKey { get; set; }

    #endregion

    #region Public Methods and Operators

    IRestClient GetInstance(Uri uri);

    IRestClient GetInstance(string uri);

    IRestClient GetInstance(RestClientAutologConfiguration configuration);

    IRestClient GetInstance();

    #endregion
  }

  public class RestClientFactory : IRestClientFactory
  {
    #region Public Properties

    public string AccountId { get; set; }
    public string RequestKey { get; set; }

    #endregion

    #region Public Methods and Operators

    public IRestClient GetInstance(Uri uri) =>
      new RestClientAutolog(uri)
        .WithHeaders(RequestKey, AccountId);

    public IRestClient GetInstance(string uri) =>
      new RestClientAutolog(uri)
        .WithHeaders(RequestKey, AccountId);

    public IRestClient GetInstance(RestClientAutologConfiguration configuration) =>
      new RestClientAutolog(configuration)
        .WithHeaders(RequestKey, AccountId);

    public IRestClient GetInstance() =>
      new RestClientAutolog()
        .WithHeaders(RequestKey, AccountId);

    #endregion
  }
}