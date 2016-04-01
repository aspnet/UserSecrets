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
        private const string ProjectPathKey = "ProjectPath";

        /// <summary>
        /// Gets the project path
        /// </summary>
        /// <param name="configurationBuilder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <returns>The project path.</returns>
        public static string GetProjectPath(this IConfigurationBuilder configurationBuilder)
        {
            if (configurationBuilder == null)
            {
                throw new ArgumentNullException(nameof(configurationBuilder));
            }

            object projectPath;
            if (configurationBuilder.Properties.TryGetValue(ProjectPathKey, out projectPath))
            {
                return (string)projectPath;
            }

#if NET451
            var stringBasePath = AppDomain.CurrentDomain.GetData("APP_CONTEXT_BASE_DIRECTORY") as string ??
                AppDomain.CurrentDomain.BaseDirectory ??
                string.Empty;

            return Path.GetFullPath(stringBasePath);
#else
            return AppContext.BaseDirectory ?? string.Empty;
#endif
        }

        /// <summary>
        /// Sets the project path.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="projectPath">The absolute path of file-based providers.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder SetProjectPath(this IConfigurationBuilder builder, string projectPath)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (projectPath == null)
            {
                throw new ArgumentNullException(nameof(projectPath));
            }

            builder.Properties[ProjectPathKey] = projectPath;
            return builder;
        }

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

            var projectPath = configuration.GetProjectPath();
            if (string.IsNullOrEmpty(projectPath))
            {
                throw new InvalidOperationException(Resources.FormatError_MissingBasePath(
                        projectPath,
                        typeof(IConfigurationBuilder).Name,
                        ProjectPathKey));
            }
            return AddSecretsFile(configuration, PathHelper.GetSecretsPath(projectPath));
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