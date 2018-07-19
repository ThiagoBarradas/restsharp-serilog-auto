using Serilog;
using Serilog.Events;
using System;

namespace RestSharp.Serilog.Auto.ConsoleTest
{
    static class Program
    {
        static void Main(string[] args)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Error)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("Domain", "MyTeam")
                .Enrich.WithProperty("Application", "MyProject")
                .WriteTo.Seq("http://localhost:5341")
                .WriteTo.Console();

            Log.Logger = loggerConfiguration.CreateLogger();

            //RestClientAutologConfiguration restClientAutologConfiguration = new RestClientAutologConfiguration()
            //{
            //    LoggerConfiguration = loggerConfiguration
            //};

            IRestClient client = new RestClientAutolog("http://pruu.herokuapp.com");
            client.AddDefaultHeader("DefaultHeaderTest", "SomeValue");

            RestRequest request = new RestRequest("dump/{name}", Method.POST);
            request.AddHeader("RequestCustomHeader", "SomeValue2");
            request.AddUrlSegment("name", "RestsharpAutoLog");
            request.AddQueryParameter("CustomQuery", "SomeValue3");
            request.AddJsonBody(new
            {
                TestOne = 123,
                TestTwo = "SomeValueBody"
            });

            var response = client.Execute(request);

            Console.ReadKey();
        }
    }
}
