[![Codacy Badge](https://api.codacy.com/project/badge/Grade/080b586bd9084d5884e66d6773bc7b26)](https://www.codacy.com/app/ThiagoBarradas/restsharp-serilog-auto?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=ThiagoBarradas/restsharp-serilog-auto&amp;utm_campaign=Badge_Grade)
[![Build status](https://ci.appveyor.com/api/projects/status/p6x1nmjhlw6jcxye/branch/master?svg=true)](https://ci.appveyor.com/project/ThiagoBarradas/restsharp-serilog-auto/branch/master)
[![codecov](https://codecov.io/gh/ThiagoBarradas/restsharp-serilog-auto/branch/master/graph/badge.svg)](https://codecov.io/gh/ThiagoBarradas/restsharp-serilog-auto)
[![NuGet Downloads](https://img.shields.io/nuget/dt/RestSharp.Serilog.Auto.svg)](https://www.nuget.org/packages/RestSharp.Serilog.Auto/)
[![NuGet Version](https://img.shields.io/nuget/v/RestSharp.Serilog.Auto.svg)](https://www.nuget.org/packages/RestSharp.Serilog.Auto/)

# RestSharp.Serilog.Auto

Do you need log all communication made with RestSharp using your serilog configuration? Just install this package and register our client proxy for `IRestClient`.

```c#
IRestClient client = new RestClientAutolog("http://www.github.com");
```

## Install via NuGet

```
PM> Install-Package RestSharp.Serilog.Auto
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

## Variables to use in messages templates

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

## How can I contribute?
Please, refer to [CONTRIBUTING](.github/CONTRIBUTING.md)

## Found something strange or need a new feature?
Open a new Issue following our issue template [ISSUE_TEMPLATE](.github/ISSUE_TEMPLATE.md)

## Changelog
See in [nuget version history](https://www.nuget.org/packages/RestSharp.Serilog.Auto)

## Did you like it? Please, make a donate :)

if you liked this project, please make a contribution and help to keep this and other initiatives, send me some Satochis.

BTC Wallet: `1G535x1rYdMo9CNdTGK3eG6XJddBHdaqfX`

![1G535x1rYdMo9CNdTGK3eG6XJddBHdaqfX](https://i.imgur.com/mN7ueoE.png)
