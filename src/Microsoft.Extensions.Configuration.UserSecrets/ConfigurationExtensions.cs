// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Configuration.UserSecrets.Internal;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Configuration extensions for adding user secrets configuration source.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Adds the user secrets configuration source.
        /// </summary>
        /// <exception cref="System.PlatformNotSupportedException">
        /// Platforms that do not support System.Reflection.Assembly.GetEntryAssembly()
        /// </exception>
        /// <param name="configuration"></param>
        /// <returns></returns>
#if NETSTANDARD1_3
        [Obsolete("This method will always throw on this platform. Use the AddUserSecrets(System.Reflection.Assembly) overload instead.")]
#endif
        public static IConfigurationBuilder AddUserSecrets(this IConfigurationBuilder configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

#if NETSTANDARD1_3
            // this was added because the version 1.0.0, netstandard1.3 version shipped with this API
            // but did not need to use GetEntryAssembly (netstandard1.5 and up).
            throw new PlatformNotSupportedException(Resources.Error_EntryAssembly_NotAvailable);
#else
            // Assembly.GetEntryAssembly requires netstandard1.5 or desktop .NET
            return AddUserSecrets(configuration, Assembly.GetEntryAssembly());
#endif
        }

        /// <summary>
        /// Adds the user secrets configuration source.
        /// </summary>
        /// <remarks>
        /// The assembly containing <typeparamref name="TStartup" /> should define an instance of <see cref="UserSecretsIdentifierFileNameAttribute" />
        /// </remarks>
        /// <typeparam name="TStartup">The startup type to use</typeparam>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IConfigurationBuilder AddUserSecrets<TStartup>(this IConfigurationBuilder configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return AddUserSecrets(configuration, typeof(TStartup).GetTypeInfo().Assembly);
        }

        /// <summary>
        /// Adds the user secrets configuration source.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="assembly">The assembly with the <see cref="UserSecretsIdentifierFileNameAttribute" /></param>
        /// <returns></returns>
        public static IConfigurationBuilder AddUserSecrets(this IConfigurationBuilder configuration, Assembly assembly)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var fileProvider = configuration.GetFileProvider();
            var filename = assembly.GetUserSecretsFileNameOrDefault();

            return AddSecretsFile(configuration, PathHelper.GetSecretsPath(fileProvider, filename));
        }

        /// <summary>
        /// Adds the user secrets configuration source with specified secrets id.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="userSecretsId"></param>
        /// <returns></returns>
        public static IConfigurationBuilder AddUserSecrets(this IConfigurationBuilder configuration, string userSecretsId)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (userSecretsId == null)
            {
                throw new ArgumentNullException(nameof(userSecretsId));
            }

            return AddSecretsFile(configuration, PathHelper.GetSecretsPathFromSecretsId(userSecretsId));
        }

        private static IConfigurationBuilder AddSecretsFile(IConfigurationBuilder configuration, string secretPath)
        {
            var directoryPath = Path.GetDirectoryName(secretPath);
            var fileProvider = Directory.Exists(directoryPath)
                ? new PhysicalFileProvider(directoryPath)
                : null;
            return configuration.AddJsonFile(fileProvider, PathHelper.Secrets_File_Name, optional: true, reloadOnChange: false);
        }
    }
}