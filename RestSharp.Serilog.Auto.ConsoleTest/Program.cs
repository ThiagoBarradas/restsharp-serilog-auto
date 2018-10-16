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
                .Enrich.WithProperty("Domain", "MyTeamRS")
                .Enrich.WithProperty("Application", "MyProjectRS")
                .WriteTo.Seq("http://localhost:5341")
                .WriteTo.Console();

            Log.Logger = loggerConfiguration.CreateLogger();

            //RestClientAutologConfiguration restClientAutologConfiguration = new RestClientAutologConfiguration()
            //{
            //    LoggerConfiguration = loggerConfiguration
            //};

            RestClientAutolog client = new RestClientAutolog("http://pruu.herokuapp.com");
            client.AddDefaultHeader("DefaultHeaderTest", "SomeValue");

            client.Configuration.JsonBlacklist = new string[] { "*password" };

            RestRequest request = new RestRequest("dump/{name}", Method.POST);
            request.AddHeader("RequestCustomHeader", "SomeValue2");
            request.AddHeader("RequestCustomHeader", "SomeValue3");
            request.AddHeader("RequestKey", "keykeykey");
            request.AddUrlSegment("name", "RestsharpAutoLog");
            request.AddQueryParameter("CustomQuery", "SomeValue3");
            request.AddQueryParameter("CustomQuery", "SomeValue2");
            request.AddQueryParameter("CustomQuery1", "SomeValue1");
            request.AddQueryParameter("CustomQuery2", "");
            request.AddQueryParameter("CustomQuery3", null);
            request.AddJsonBody(new
            {
                TestOne = 123,
                TestTwo = "SomeValueBody",
                Password = "12321312",
                Test = new {
                    Password = 123
                }
            });

            var response = client.Execute(request);

            Console.ReadKey();
        }
    }
}
