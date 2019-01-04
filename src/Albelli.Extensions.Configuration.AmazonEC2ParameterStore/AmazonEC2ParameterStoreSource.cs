using System;
using Amazon;
using Amazon.Runtime;
using Amazon.SimpleSystemsManagement;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Albelli.Extensions.Configuration.AmazonEC2ParameterStore
{
    internal sealed class AmazonEC2ParameterStoreSource : IConfigurationSource
    {
        private readonly bool parseStringListAsList;

        private readonly bool failIfCantLoad;

        public AmazonEC2ParameterStoreSource(
            [NotNull] string rootPath,
            [NotNull] RegionEndpoint region,
            bool parseStringListAsList,
            bool failIfCantLoad,
            [CanBeNull] ILoggerFactory loggerFactory)
            : this(new AmazonSimpleSystemsManagementClient(region), rootPath, parseStringListAsList, failIfCantLoad, loggerFactory) { }

        public AmazonEC2ParameterStoreSource(
            [NotNull] AWSCredentials credentials,
            [NotNull] string rootPath,
            [NotNull] RegionEndpoint region,
            bool parseStringListAsList,
            bool failIfCantLoad,
            [CanBeNull] ILoggerFactory loggerFactory)
        : this(new AmazonSimpleSystemsManagementClient(credentials, region), rootPath, parseStringListAsList, failIfCantLoad, loggerFactory) { }

        private AmazonEC2ParameterStoreSource(
            [NotNull] IAmazonSimpleSystemsManagement amazonSimpleSystemsManagement,
            [NotNull] string rootPath,
            bool parseStringListAsList,
            bool failIfCantLoad,
            [CanBeNull] ILoggerFactory loggerFactory)
        {
            this.RootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
            this.AmazonSimpleSystemsManagement = amazonSimpleSystemsManagement ?? throw new ArgumentNullException(nameof(amazonSimpleSystemsManagement));

            this.LoggerFactory = loggerFactory;
            this.parseStringListAsList = parseStringListAsList;
            this.failIfCantLoad = failIfCantLoad;
        }

        /// <summary>
        /// A root directory to load settings from.
        /// </summary>
        public string RootPath { get; }

        /// <summary>
        /// Logger factory to be passed to the provider.
        /// </summary>
        private ILoggerFactory LoggerFactory { get; }

        /// <summary>
        /// AmazonSimpleSystemsManagement service to be passed to the provider.
        /// </summary>
        private IAmazonSimpleSystemsManagement AmazonSimpleSystemsManagement { get; }

        /// <summary>
        /// Builds the <see cref="AmazonEC2ParameterStoreProvider"/> for this source.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/>The configuration builder.</param>
        /// <returns>A <see cref="AmazonEC2ParameterStoreProvider"/>The EC2 ParameterStore provider.</returns>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new AmazonEC2ParameterStoreProvider(this.AmazonSimpleSystemsManagement, this.RootPath, this.parseStringListAsList, this.failIfCantLoad, this.LoggerFactory);
        }
    }
}
