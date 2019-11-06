
# RestSharp.Serilog.Auto

Do you need log all communication made with RestSharp using your serilog configuration? Just install this package and register our client proxy for `IRestClient`.

```c#
IRestClient client = new RestClientAutolog("http://www.github.com");
```

## Install via NuGet

```
PM> Install-Package RestSharp.Serilog.Auto.Extented
```

## How to use

You can change error message, success message and logger configuration. 

```c#

var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("Domain", "MyDomain")
                .Enrich.WithProperty("Application", "MyProject")
                .WriteTo.Seq("http://localhost:5341")
                .WriteTo.Console();

var restClientAutologConfiguration = new RestClientAutologConfiguration()
{
    MessageTemplateForSuccess = "{Method} {Uri} responded {StatusCode}", 
    MessageTemplateForError = "{Method} {Uri} is not good! {ErrorMessage}", 
    LoggerConfiguration = loggerConfiguration
};

IRestClient client = new RestClientAutolog("http://www.github.com", restClientAutologConfiguration);
```

Serilog uses `Log.Logger` as global. If you setup this on your application Startup/Bootstrap, it's not needed change logger configuration.

## Variables to use in message templates

Properties created like `(...).Enrich.WithProperty("Application", "MyProject")` can be used in templates.

Default variables:

* `Agent`
* `ElapsedMilliseconds`
* `Method`
* `Uri`
* `Host`
* `Path`
* `Query`
* `Body`
* `RequestHeaders`
* `StatusCode`
* `StatusDescription`
* `ResponseStatus`
* `ProtocolVersion`
* `IsSuccessful`
* `ErrorMessage`
* `ErrorException`
* `Content`
* `ContentEncoding`
* `ContentLength`
* `ContentType`
* `ResponseHeaders`
