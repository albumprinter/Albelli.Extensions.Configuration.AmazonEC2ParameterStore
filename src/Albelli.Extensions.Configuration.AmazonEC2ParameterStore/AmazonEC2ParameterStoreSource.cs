using System;
using Amazon;
using Amazon.SimpleSystemsManagement;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Albelli.Extensions.Configuration.AmazonEC2ParameterStore
{
    internal sealed class AmazonEC2ParameterStoreSource : IConfigurationSource
    {
        private readonly bool parseStringListAsList;

        public AmazonEC2ParameterStoreSource([NotNull] ILoggerFactory loggerFactory, [NotNull] string rootPath, [NotNull] string regionName)
            : this(loggerFactory, rootPath, RegionEndpoint.GetBySystemName(regionName)) { }

        public AmazonEC2ParameterStoreSource([NotNull] ILoggerFactory loggerFactory, [NotNull] string rootPath, [NotNull] RegionEndpoint region)
            : this(loggerFactory, rootPath, region, false) { }

        public AmazonEC2ParameterStoreSource([NotNull] ILoggerFactory loggerFactory, [NotNull] string rootPath, [NotNull] RegionEndpoint region, bool parseStringListAsList)
        {
            this.LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            this.RootPath = rootPath ?? throw new ArgumentNullException(nameof(loggerFactory));

            this.AmazonSimpleSystemsManagement = new AmazonSimpleSystemsManagementClient(region);

            this.parseStringListAsList = parseStringListAsList;
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
            return new AmazonEC2ParameterStoreProvider(this.LoggerFactory, this.AmazonSimpleSystemsManagement, this.RootPath, this.parseStringListAsList);
        }
    }
}
