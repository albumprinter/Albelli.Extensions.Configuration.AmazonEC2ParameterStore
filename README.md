# AmazonEC2ParameterStore

[![Build status](https://ci.appveyor.com/api/projects/status/f6cm4vwg9i4js7q0?svg=true)](https://ci.appveyor.com/project/albumprinter/albelli-extensions-configuration-amazonec2paramete)
[![Nuget](https://img.shields.io/nuget/v/Albelli.Extensions.Configuration.AmazonEC2ParameterStore.svg)](https://www.nuget.org/packages/Albelli.Extensions.Configuration.AmazonEC2ParameterStore/)

This .NET Core Configuration Provider is used to load settings from [Amazon EC2 ParameterStore](AmazonEC2ParameterStore) directory.

Parameter names are being transformed by replacement `/` with `:` to be compatible with the format that the configuration library expects. 

For example:
`/stage/api-lambda/s3/bucketName` becomes `stage:api-lambda:s3:bucketName`

So
`Configuration.GetSection("stage:api-lambda:s3")` and `Configuration.GetValue("stage:api-lambda:s3:bucketName")` should work as expected. 

If you like to use `Configuration.GetSection(..).Bind()` or `Configuration.Bind()` - parameter names should not contain any extra characters, like `.-_`.

Example usage:
(please note that `Get<>` extension is already available in .net core 2.0 and should be used instead of the one given in the example below)

```
namespace Albelli.AwesomeApi
{
    public interface IMyAppSettings
    {
        string ValueA { get; set;}
    }

    public class MyAppSettings: IMyAppSettings
    {
        string ValueA { get; set;}
    }

    public static class ConfigurationExtensions
    {
        public static T Get<T>(this IConfigurationRoot configuration)
            where T : class, new()
        {
            var settings = new T();
            configuration.Bind(settings);
            return settings;
        }

        public static T Get<T>(this IConfigurationSection section)
            where T : class, new()
        {
            var settings = new T();
            section.Bind(settings);
            return settings;
        }
    }

    public sealed class Startup
    {
        private readonly string env;
        private readonly string functionName;

        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory, ILogger<Startup> logger)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();

            this.env = "prod";
            this.functionName = "my_lambda_function";

            var configPath = $"/{this.env}/{this.functionName}";

            builder.AddEC2ParameterStoreVariables(
               loggerFactory,
               configPath,
               "eu-west-1");

            this.Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<IMyAppSettings>(
                    context => this.Configuration.GetSection($"{this.env}:{this.functionName}:settings").Get<MyAppSettings>())
                .AddMvc()
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseMvc();
        }
    }
}

```

[AmazonEC2ParameterStore]: http://docs.aws.amazon.com/systems-manager/latest/userguide/sysman-paramstore-working.html


  