// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.Extensions.Configuration
{
    public static class ConfigurationExtensions
    {
        private const string Secrets_File_Name = "secrets.json";

        /// <summary>
        /// Adds the user secrets configuration source.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IConfigurationBuilder AddUserSecrets(this IConfigurationBuilder configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (string.IsNullOrEmpty(configuration.GetBasePath()))
            {
                throw new InvalidOperationException(Resources.FormatError_MissingBasePath(
                    configuration.GetBasePath(),
                    typeof(IConfigurationBuilder).Name,
                    "BasePath"));
            }

            return AddSecretsFile(configuration, PathHelper.GetSecretsPath(configuration.GetBasePath()));
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
            return configuration.AddJsonFile(source => {
                source.Path = PathHelper.Secrets_File_Name;
                if (Directory.Exists(directoryPath))
                {
                    source.FileProvider = new PhysicalFileProvider(directoryPath);
                }
                source.Optional = true;
            });
        }
    }
}