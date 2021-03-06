﻿using System;

namespace RestSharp.Serilog.Auto
{
    public interface IRestClientFactory
    {
        IRestClient GetInstance(Uri uri);

        IRestClient GetInstance(string uri);

        IRestClient GetInstance(RestClientAutologConfiguration configuration);

        IRestClient GetInstance();
    }

    public class RestClientFactory : IRestClientFactory
    {
        public IRestClient GetInstance(Uri uri)
        {
            return new RestClientAutolog(uri);
        }

        public IRestClient GetInstance(string uri)
        {
            return new RestClientAutolog(uri);
        }

        public IRestClient GetInstance(RestClientAutologConfiguration configuration)
        {
            return new RestClientAutolog(configuration);
        }

        public IRestClient GetInstance()
        {
            return new RestClientAutolog();
        }
    }
}
