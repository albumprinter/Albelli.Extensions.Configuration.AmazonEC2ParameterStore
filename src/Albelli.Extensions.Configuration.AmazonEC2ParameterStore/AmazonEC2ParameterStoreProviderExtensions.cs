using System;
using Amazon;
using Amazon.Runtime;
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
        /// <param name="credentials">AWS Credentials. If null, then the default fall back is used.</param>
        /// <param name="parseStringListAsList">If set to true, parses the comma deilimited value into multiple values so it can be mapped to lists</param>
        /// <param name="failIfCantLoad">Throw an exception if parameter store variables fail to load</param>
        /// <returns>The <see cref="AmazonEC2ParameterStoreSource"/>.</returns>
        [PublicAPI]
        public static IConfigurationBuilder AddEC2ParameterStoreVariables(
            [NotNull] this IConfigurationBuilder configurationBuilder,
            [NotNull] string rootPath,
            [NotNull] string region,
            [CanBeNull] ILoggerFactory loggerFactory = null,
            [CanBeNull] AWSCredentials credentials = null,
            bool parseStringListAsList = false,
            bool failIfCantLoad = false)
        {
            return configurationBuilder.AddEC2ParameterStoreVariables(
                rootPath,
                RegionEndpoint.GetBySystemName(region),
                loggerFactory,
                credentials,
                parseStringListAsList);
        }

        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> 
        /// that reads configuration values from AWS EC2 ParameterStore and decrypt them using AWS KMS.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="loggerFactory">Logger factory to be passed to the provider.</param>
        /// <param name="rootPath">Parameters directory to load from.</param>
        /// <param name="region">Parameters AWS region.</param>
        /// <param name="credentials">AWS Credentials. If null, then the default fall back is used.</param>
        /// <param name="parseStringListAsList">If set to true, parses the comma deilimited value into multiple values so it can be mapped to lists</param>
        /// <param name="failIfCantLoad">Throw an exception if parameter store variables fail to load</param>
        /// <returns>The <see cref="AmazonEC2ParameterStoreSource"/>.</returns>
        [PublicAPI]
        public static IConfigurationBuilder AddEC2ParameterStoreVariables(
            [NotNull] this IConfigurationBuilder configurationBuilder,
            [NotNull] string rootPath,
            [NotNull] RegionEndpoint region,
            [CanBeNull] ILoggerFactory loggerFactory = null,
            [CanBeNull] AWSCredentials credentials = null,
            bool parseStringListAsList = false,
            bool failIfCantLoad = false)
        {
            return
                credentials == null
                    ? configurationBuilder.Add(new AmazonEC2ParameterStoreSource(rootPath, region, parseStringListAsList, failIfCantLoad, loggerFactory))
                    : configurationBuilder.Add(new AmazonEC2ParameterStoreSource(credentials, rootPath, region, parseStringListAsList, failIfCantLoad, loggerFactory));
        }

        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> 
        /// that reads configuration values from AWS EC2 ParameterStore and decrypt them using AWS KMS.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="loggerFactory">Logger factory to be passed to the provider.</param>
        /// <param name="rootPath">Parameters directory to load from.</param>
        /// <param name="region">Parameters AWS region.</param>
        /// <param name="credentials">AWS Credentials. If null, then the default fall back is used.</param>
        /// <param name="parseStringListAsList">If set to true, parses the comma deilimited value into multiple values so it can be mapped to lists</param>
        /// <returns>The <see cref="AmazonEC2ParameterStoreSource"/>.</returns>
        [PublicAPI]
        [Obsolete("Please use overloads where ILoggerFactory is optional")]
        public static IConfigurationBuilder AddEC2ParameterStoreVariables(
            [NotNull] this IConfigurationBuilder configurationBuilder,
            [CanBeNull] ILoggerFactory loggerFactory,
            [NotNull] string rootPath,
            [NotNull] string region,
            [CanBeNull] AWSCredentials credentials = null,
            bool parseStringListAsList = false)
        {
            return configurationBuilder.AddEC2ParameterStoreVariables(
                rootPath,
                RegionEndpoint.GetBySystemName(region),
                loggerFactory,
                credentials,
                parseStringListAsList);
        }

        /// <summary>
        /// Adds an <see cref="IConfigurationProvider"/> 
        /// that reads configuration values from AWS EC2 ParameterStore and decrypt them using AWS KMS.
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="loggerFactory">Logger factory to be passed to the provider.</param>
        /// <param name="rootPath">Parameters directory to load from.</param>
        /// <param name="region">Parameters AWS region.</param>
        /// <param name="credentials">AWS Credentials. If null, then the default fall back is used.</param>
        /// <param name="parseStringListAsList">If set to true, parses the comma deilimited value into multiple values so it can be mapped to lists</param>
        /// <returns>The <see cref="AmazonEC2ParameterStoreSource"/>.</returns>
        [PublicAPI]
        [Obsolete("Please use overloads where ILoggerFactory is optional")]
        public static IConfigurationBuilder AddEC2ParameterStoreVariables(
            [NotNull] this IConfigurationBuilder configurationBuilder,
            [CanBeNull] ILoggerFactory loggerFactory,
            [NotNull] string rootPath,
            [NotNull] RegionEndpoint region,
            [CanBeNull] AWSCredentials credentials = null,
            bool parseStringListAsList = false)
        {
            return configurationBuilder.AddEC2ParameterStoreVariables(
                rootPath, 
                region, 
                loggerFactory, 
                credentials, 
                parseStringListAsList);
        }
    }
}
