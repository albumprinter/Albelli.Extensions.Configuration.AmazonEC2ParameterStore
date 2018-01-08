using System;
using Amazon;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Albelli.Extensions.Configuration.AmazonEC2ParameterStore
{
    public static class AmazonEC2ParameterStoreProviderExtensions
    {
        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> 
        /// that reads configuration values from AWS EC2 ParameterStore and decrypt them using AWS KMS.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="loggerFactory">Logger factory to be passed to the provider.</param>
        /// <param name="rootPath">Parameters directory to load from.</param>
        /// <param name="region">Parameters AWS region.</param>
        /// <returns>The <see cref="AmazonEC2ParameterStoreSource"/>.</returns>
        public static IConfigurationBuilder AddEC2ParameterStoreVariables(
            [NotNull] this IConfigurationBuilder configurationBuilder,
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] string rootPath,
            [NotNull] string region)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            return configurationBuilder.Add(new AmazonEC2ParameterStoreSource(loggerFactory, rootPath, region));
        }

        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> 
        /// that reads configuration values from AWS EC2 ParameterStore and decrypt them using AWS KMS.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="loggerFactory">Logger factory to be passed to the provider.</param>
        /// <param name="rootPath">Parameters directory to load from.</param>
        /// <param name="region">Parameters AWS region.</param>
        /// <returns>The <see cref="AmazonEC2ParameterStoreSource"/>.</returns>
        public static IConfigurationBuilder AddEC2ParameterStoreVariables(
            [NotNull] this IConfigurationBuilder configurationBuilder,
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] string rootPath,
            [NotNull] RegionEndpoint region)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            return configurationBuilder.Add(new AmazonEC2ParameterStoreSource(loggerFactory, rootPath, region));
        }
    }
}
