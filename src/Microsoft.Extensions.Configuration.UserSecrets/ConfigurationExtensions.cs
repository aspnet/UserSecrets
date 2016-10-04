// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.Extensions.Configuration
{
    /// <summary>
    /// Configuration extensions for adding user secrets configuration source.
    /// </summary>
    public static class ConfigurationExtensions
    {
        private const string Secrets_File_Name = "secrets.json";

        /// <summary>
        ///     <para>
        ///     This method is obsolete and will be removed in a future version. The recommended alternative is Microsoft.Extensions.Configuration.ConfigurationExtensions.AddUserSecrets().
        ///     </para>
        ///     <para>
        ///     Adds the users secrets configuration source using 'project.json' to look up the user secrets id.
        ///     </para>
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        [Obsolete("This method is obsolete and will be removed in a future version. The recommended alternative is Microsoft.Extensions.Configuration.ConfigurationExtensions.AddUserSecrets().")]
        public static IConfigurationBuilder AddUserSecretsFromProjectJson(this IConfigurationBuilder configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var fileProvider = configuration.GetFileProvider();

#pragma warning disable CS0618
            return AddSecretsFile(configuration, PathHelper.GetSecretsPath(fileProvider));
#pragma warning restore CS0618
        }

        /// <summary>
        /// Adds the user secrets configuration source. Searches the assembly from <see cref="Assembly.GetEntryAssembly"/>
        /// for an instance of <see cref="UserSecretsIdAttribute"/>
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IConfigurationBuilder AddUserSecrets(this IConfigurationBuilder configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return AddUserSecrets(configuration, Assembly.GetEntryAssembly());
        }

        /// <summary>
        /// Adds the user secrets configuration source.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="assembly">The assembly with the <see cref="UserSecretsIdAttribute" /></param>
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

            var id = assembly.GetCustomAttribute<UserSecretsIdAttribute>();
            if (id == null)
            {
                throw new ArgumentException(Resources.FormatError_Missing_UserSecretsIdAttribute(assembly.FullName) ,nameof(assembly));
            }

            return AddUserSecrets(configuration, id.UserSecretsId);
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