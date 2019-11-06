namespace RestSharp.Serilog.Auto
{
    using System;

    /// <summary>
    /// Defines the <see cref="IRestClientFactory" />
    /// </summary>
    public interface IRestClientFactory
    {
        /// <summary>
        /// The GetInstance
        /// </summary>
        /// <returns>The <see cref="IRestClient"/></returns>
        IRestClient GetInstance();

        /// <summary>
        /// The GetInstance
        /// </summary>
        /// <param name="configuration">The configuration<see cref="RestClientAutologConfiguration"/></param>
        /// <returns>The <see cref="IRestClient"/></returns>
        IRestClient GetInstance(RestClientAutologConfiguration configuration);

        /// <summary>
        /// The GetInstance
        /// </summary>
        /// <param name="uri">The uri<see cref="string"/></param>
        /// <returns>The <see cref="IRestClient"/></returns>
        IRestClient GetInstance(string uri);

        /// <summary>
        /// The GetInstance
        /// </summary>
        /// <param name="uri">The uri<see cref="Uri"/></param>
        /// <returns>The <see cref="IRestClient"/></returns>
        IRestClient GetInstance(Uri uri);
    }

    /// <summary>
    /// Defines the <see cref="RestClientFactory" />
    /// </summary>
    public class RestClientFactory : IRestClientFactory
    {
        /// <summary>
        /// The GetInstance
        /// </summary>
        /// <returns>The <see cref="IRestClient"/></returns>
        public IRestClient GetInstance()
        {
            return new RestClientAutolog();
        }

        /// <summary>
        /// The GetInstance
        /// </summary>
        /// <param name="configuration">The configuration<see cref="RestClientAutologConfiguration"/></param>
        /// <returns>The <see cref="IRestClient"/></returns>
        public IRestClient GetInstance(RestClientAutologConfiguration configuration)
        {
            return new RestClientAutolog(configuration);
        }

        /// <summary>
        /// The GetInstance
        /// </summary>
        /// <param name="uri">The uri<see cref="string"/></param>
        /// <returns>The <see cref="IRestClient"/></returns>
        public IRestClient GetInstance(string uri)
        {
            return new RestClientAutolog(uri);
        }

        /// <summary>
        /// The GetInstance
        /// </summary>
        /// <param name="uri">The uri<see cref="Uri"/></param>
        /// <returns>The <see cref="IRestClient"/></returns>
        public IRestClient GetInstance(Uri uri)
        {
            return new RestClientAutolog(uri);
        }
    }
}
