using System;

namespace RestSharp.Serilog.Auto
{
    public interface IRestClientFactory
    {
        string RequestKey { get; set; }

        string AccountId { get; set; }

        IRestClient GetInstance(Uri uri);

        IRestClient GetInstance(string uri);

        IRestClient GetInstance(RestClientAutologConfiguration configuration);

        IRestClient GetInstance();
    }

    public class RestClientFactory : IRestClientFactory
    {
        public string RequestKey { get; set; }

        public string AccountId { get; set; }

        public IRestClient GetInstance(Uri uri)
        {
            return new RestClientAutolog(uri)
                .WithHeaders(this.RequestKey, this.AccountId);
        }

        public IRestClient GetInstance(string uri)
        {
            return new RestClientAutolog(uri)
                .WithHeaders(this.RequestKey, this.AccountId);
        }

        public IRestClient GetInstance(RestClientAutologConfiguration configuration)
        {
            return new RestClientAutolog(configuration)
                .WithHeaders(this.RequestKey, this.AccountId);
        }

        public IRestClient GetInstance()
        {
            return new RestClientAutolog()
                .WithHeaders(this.RequestKey, this.AccountId);
        }
    }
}
