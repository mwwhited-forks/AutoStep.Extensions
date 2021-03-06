﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AutoStep.Extensions.NuGetExtensions;
using Microsoft.Extensions.Logging;

namespace AutoStep.Extensions
{
    /// <summary>
    /// Entry point for loading extensions.
    /// </summary>
    public class ExtensionSetLoader
    {
        private readonly IHostContext hostContext;
        private readonly ILogger<ExtensionSetLoader> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionSetLoader"/> class.
        /// </summary>
        /// <param name="environment">The host environment.</param>
        /// <param name="loggerFactory">A logger factory to which the extension load process will write information.</param>
        /// <param name="extensionPackageTag">An optional package tag to filter the set of nuget packages that will be considered as 'entry points', and therefore implicitly loaded.</param>
        public ExtensionSetLoader(IAutoStepEnvironment environment, ILoggerFactory loggerFactory, string? extensionPackageTag)
        {
            var hostAssembly = typeof(ExtensionSetLoader).Assembly;

            // Need the dependency context for the host assembly, create a host context from that.
            hostContext = new HostContext(hostAssembly, environment, extensionPackageTag);

            logger = loggerFactory.CreateLogger<ExtensionSetLoader>();
        }

        /// <summary>
        /// Load a set of extensions.
        /// </summary>
        /// <param name="sourceSettings">The source settings, dictating where packages come from.</param>
        /// <param name="packageExtensions">A set of package extension configurations, specifying which extensions to install.</param>
        /// <param name="localExtensions">A set of local extension configurations, specifying which folders to load local extensions from.</param>
        /// <param name="noCache">If set to true, then any existing caches will be ignored, and the set of packages will be downloaded directly.</param>
        /// <param name="localExtensionDebugBuilds">If set to true, then the Debug configuration of local extensions will be used, instead of Release.</param>
        /// <param name="cancelToken">A cancellation token to abort the process.</param>
        /// <returns>An awaitable task, containing the set of resolved packages.</returns>
        public ValueTask<IInstallablePackageSet> ResolveExtensionsAsync(
            ISourceSettings sourceSettings,
            IEnumerable<PackageExtensionConfiguration> packageExtensions,
            IEnumerable<FolderExtensionConfiguration> localExtensions,
            bool noCache,
            bool localExtensionDebugBuilds,
            CancellationToken cancelToken)
        {
            if (sourceSettings is null)
            {
                throw new ArgumentNullException(nameof(sourceSettings));
            }

            if (packageExtensions is null)
            {
                throw new ArgumentNullException(nameof(packageExtensions));
            }

            if (localExtensions is null)
            {
                throw new ArgumentNullException(nameof(localExtensions));
            }

            // Ensure that the extensions directory exists.
            Directory.CreateDirectory(hostContext.Environment.ExtensionsDirectory);

            // Create composite resolver.
            var compositeResolver = new CompositeExtensionResolver(
                hostContext,
                new LocalExtensionResolver(hostContext, logger, localExtensionDebugBuilds),
                new NugetFallbackPackageResolver(sourceSettings, hostContext, noCache, logger));

            // Context object for the resolve operation.
            var extensionResolveContext = new ExtensionResolveContext(packageExtensions, localExtensions);

            return compositeResolver.ResolvePackagesAsync(extensionResolveContext, cancelToken);
        }
    }
}
