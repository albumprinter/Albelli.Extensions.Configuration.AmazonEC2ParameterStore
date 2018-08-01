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
        private readonly IAmazonSimpleSystemsManagement client;
        private readonly ILogger<AmazonEC2ParameterStoreProvider> logger;

        /// <summary>
        /// Creates a new instance of <see cref="AmazonEC2ParameterStoreProvider"/>.
        /// </summary>
        public AmazonEC2ParameterStoreProvider(
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] IAmazonSimpleSystemsManagement client,
            [NotNull] string rootPath,
            bool parseStringListAsList = false)
        {
            this.rootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.logger = loggerFactory?.CreateLogger<AmazonEC2ParameterStoreProvider>() ?? throw new ArgumentNullException(nameof(loggerFactory));

            this.parseStringListAsList = parseStringListAsList;
        }

        public override void Load()
        {
            try
            {
                this.LoadAsync().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                this.logger.LogError(0, e, "Failed to access Amazon EC2 ParameterStore");
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
                    var convertedKey = resultParameter.Name
                        .Trim('/')
                        .Replace("/", ":");

                    if (resultParameter.Type == ParameterType.SecureString)
                    {
                        this.logger.LogInformation("EC2 ParameterStore has returned {ParameterName}", resultParameter.Name);
                    }
                    else if (resultParameter.Type == ParameterType.StringList
                             && parseStringListAsList)
                    {
                        var listStrings = resultParameter.Value;

                        for (var index = 0; index < listStrings.Split(',').Length; index++)
                        {
                            var substring = listStrings.Split(',')[index];
                            var newKey = $"{convertedKey}:{index}";
                            result.Add(newKey, substring);
                        }
                    }
                    else
                    {
                        this.logger.LogInformation("EC2 ParameterStore has returned {ParameterName}  with value \'{ParameterValue}\'", resultParameter.Name, resultParameter.Value);
                    }

                    result.Add(convertedKey, resultParameter.Value);
                }

                request.NextToken = response.NextToken;

            } while (!string.IsNullOrEmpty(request.NextToken));

            this.logger.LogInformation(
                "EC2 ParameterStore has returned {TotalParametersCount} parameters in total",
                result.Count);

            return result;
        }
    }
}
