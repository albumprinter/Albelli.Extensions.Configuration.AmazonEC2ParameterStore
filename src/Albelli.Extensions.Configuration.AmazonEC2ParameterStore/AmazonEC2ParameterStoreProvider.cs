using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Albelli.Extensions.Configuration.AmazonEC2ParameterStore
{
    public sealed class AmazonEC2ParameterStoreProvider : ConfigurationProvider
    {
        private readonly string rootPath;
        private readonly bool parseStringListAsList;
        private readonly bool failIfCantLoad;
        private readonly IAmazonSimpleSystemsManagement client;
        private readonly ILogger<AmazonEC2ParameterStoreProvider> logger;

        /// <summary>
        /// Creates a new instance of <see cref="AmazonEC2ParameterStoreProvider"/>.
        /// </summary>
        public AmazonEC2ParameterStoreProvider(
            [NotNull] IAmazonSimpleSystemsManagement client,
            [NotNull] string rootPath,
            bool parseStringListAsList = false,
            bool failIfCantLoad = false,
            [CanBeNull] ILoggerFactory loggerFactory = null)
        {
            this.rootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
            this.client = client ?? throw new ArgumentNullException(nameof(client));

            if (loggerFactory == null)
            {
                loggerFactory = new LoggerFactory();
            }

            this.logger = loggerFactory.CreateLogger<AmazonEC2ParameterStoreProvider>();

            this.parseStringListAsList = parseStringListAsList;
            this.failIfCantLoad = failIfCantLoad;
        }

        /// <summary>
        /// Creates a new instance of <see cref="AmazonEC2ParameterStoreProvider"/>.
        /// </summary>
        [Obsolete("Please use the overload where ILoggerFactory is optional")]
        [PublicAPI]
        public AmazonEC2ParameterStoreProvider(
            [CanBeNull] ILoggerFactory loggerFactory,
            [NotNull] IAmazonSimpleSystemsManagement client,
            [NotNull] string rootPath,
            bool parseStringListAsList = false)
        : this(client, rootPath, parseStringListAsList, false, loggerFactory) { }

        public override void Load()
        {
            try
            {
                this.LoadAsync().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                this.logger.LogError(0, e, "Failed to access Amazon EC2 ParameterStore");

                if (failIfCantLoad)
                {
                    throw;
                }
            }
        }

        private async Task LoadAsync()
        {
            foreach (var pair in await this.GetParametersAsync(this.client, this.rootPath))
            {
                this.Data.Add(pair.Key, pair.Value);
            }
        }

        private async Task<IDictionary<string, string>> GetParametersAsync(IAmazonSimpleSystemsManagement ssm, string path)
        {
            var request = new GetParametersByPathRequest
            {
                Path = path,
                MaxResults = 10,
                WithDecryption = true,
                Recursive = true
            };

            var result = new Dictionary<string, string>();

            do
            {
                var response = await ssm.GetParametersByPathAsync(request);

                this.logger.LogInformation("EC2 ParameterStore has {ParametersCount} parameters in \'{ParametersPath}\'",
                    response.Parameters.Count,
                    path);

                foreach (var resultParameter in response.Parameters)
                {
                    LogParameter(resultParameter);

                    foreach (var kvp in BuildParameters(resultParameter, parseStringListAsList))
                    {
                        result.Add(kvp.Key, kvp.Value);
                    }
                }

                request.NextToken = response.NextToken;

            } while (!string.IsNullOrEmpty(request.NextToken));

            this.logger.LogInformation(
                "EC2 ParameterStore has returned {TotalParametersCount} parameters in total",
                result.Count);

            return result;
        }

        private void LogParameter(Parameter parameter)
        {
            if (parameter.Type == ParameterType.SecureString)
            {
                this.logger.LogInformation("EC2 ParameterStore has returned {ParameterName}", parameter.Name);
            }
            else
            {
                this.logger.LogInformation("EC2 ParameterStore has returned {ParameterName}  with value \'{ParameterValue}\'", parameter.Name, parameter.Value);
            }
        }

        private static IDictionary<string, string> BuildParameters(Parameter parameter, bool parseStringListAsList)
        {
            var results = new Dictionary<string, string>();

            var convertedKey = parameter.Name
                .Trim('/')
                .Replace("/", ":");

            if (parameter.Type == ParameterType.StringList && parseStringListAsList)
            {
                var stringListAsArray = parameter.Value.Split(',');

                for (var index = 0; index < stringListAsArray.Length; index++)
                {
                    var substring = stringListAsArray[index];
                    var newKey = $"{convertedKey}:{index}";
                    results.Add(newKey, substring);
                }
            }
            else
            {
                results.Add(convertedKey, parameter.Value);
            }


            return results;
        }
    }
}
