﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoStep.Extensions.Abstractions;
using Microsoft.Extensions.Logging;

namespace AutoStep.Extensions
{
    /// <summary>
    /// Contains the set of installed packages.
    /// </summary>
    public class InstalledExtensionPackages
    {
        public static InstalledExtensionPackages Empty { get; } = new InstalledExtensionPackages(Array.Empty<IPackageMetadata>());

        /// <summary>
        /// Initializes a new instance of the <see cref="InstalledExtensionPackages"/> class.
        /// </summary>
        /// <param name="packages">The set of package metadata.</param>
        public InstalledExtensionPackages(IReadOnlyList<IPackageMetadata> packages)
        {
            Packages = packages;
        }

        /// <summary>
        /// Gets the set of loaded package metadata.
        /// </summary>
        public IReadOnlyList<IPackageMetadata> Packages { get; }

        /// <summary>
        /// Get all packages that are considered top-level (i.e. were originally requested as configured extensions).
        /// </summary>
        /// <returns>The filtered package set.</returns>
        public IEnumerable<IPackageMetadata> GetTopLevelPackages()
        {
            return Packages.Where(x => x.IsTopLevel);
        }

        public ILoadedExtensions<TEntryPoint> LoadExtensionsFromPackages<TEntryPoint>(ILoggerFactory loggerFactory)
            where TEntryPoint : IDisposable
        {
            var loadedExtensions = new LoadedExtensions<TEntryPoint>(this);

            loadedExtensions.LoadEntryPoints(loggerFactory);

            return loadedExtensions;
        }
    }
}