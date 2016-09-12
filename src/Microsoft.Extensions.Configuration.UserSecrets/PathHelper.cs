// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration.UserSecrets.Internal;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Extensions.Configuration.UserSecrets
{
    /// <summary>
    /// Utility methods for finding user secrets paths
    /// </summary>
    public class PathHelper
    {
        internal const string Secrets_File_Name = "secrets.json";

#if NET451 || NETSTANDARD1_5
        /// <summary>
        /// Gets the path to the secrets file. Uses the <paramref name="provider" /> and <seealso cref="Assembly.GetEntryAssembly" /> to find the user secrets id.
        /// </summary>
        /// <param name="provider">The file provider</param>
        /// <returns>The filepath to secrets file</returns>
        public static string GetSecretsPath(IFileProvider provider)
        {
            return GetSecretsPath(provider, Assembly.GetEntryAssembly().GetUserSecretsFileNameOrDefault());
        }
#endif

        /// <summary>
        /// Gets the path to the secrets file. If a directory is given, finds the user secrets id in the JSON named 'project.json'.
        /// </summary>
        /// <param name="projectPath"></param>
        /// <returns>The filepath to secrets file</returns>
        // TODO remove in 2.0
        [Obsolete("This API will be removed in future releases. Use PathHelper.GetSecretsPath(rootPath, identifierFileName) instead.")]
        public static string GetSecretsPath(string projectPath)
        {
            if (projectPath == null)
            {
                throw new ArgumentNullException(nameof(projectPath));
            }

            if (Directory.Exists(projectPath))
            {
                return GetSecretsPath(projectPath, "project.json");
            }

            return GetSecretsPath(Path.GetDirectoryName(projectPath), Path.GetFileName(projectPath));
        }

        /// <summary>
        /// Gets the path to the secrets file. Finds the user secrets id in the JSON <paramref name="identifierFileName" />.
        /// </summary>
        /// <param name="rootPath">The path containing <paramref name="identifierFileName" /></param>
        /// <param name="identifierFileName">The JSON file containing the user secrets id</param>
        /// <returns>The filepath to secrets file</returns>
        public static string GetSecretsPath(string rootPath, string identifierFileName)
        {
            if (rootPath == null)
            {
                throw new ArgumentNullException(nameof(rootPath));
            }

            using (var provider = new PhysicalFileProvider(rootPath))
            {
                return GetSecretsPath(provider, identifierFileName);
            }
        }

        /// <summary>
        /// Gets the path to the secrets file. Finds the user secrets id in the JSON <paramref name="identifierFileName" />.
        /// </summary>
        /// <param name="provider">The provider containing <paramref name="identifierFileName" /></param>
        /// <param name="identifierFileName">The JSON file containing the user secrets id</param>
        /// <returns>The filepath to secrets file</returns>
        public static string GetSecretsPath(IFileProvider provider, string identifierFileName)
        {
            return GetSecretsPathFromSecretsId(GetUserSecretsIdFromFile(provider, identifierFileName));
        }

        /// <summary>
        /// Gets the path to the secrets file.
        /// </summary>
        /// <param name="userSecretsId">The user secrets id</param>
        /// <returns>The filepath to secrets file</returns>
        public static string GetSecretsPathFromSecretsId(string userSecretsId)
        {
            if (userSecretsId == null)
            {
                throw new ArgumentNullException(nameof(userSecretsId));
            }

            var badCharIndex = userSecretsId.IndexOfAny(Path.GetInvalidPathChars());
            if (badCharIndex != -1)
            {
                throw new InvalidOperationException(
                    string.Format(
                        Resources.Error_Invalid_Character_In_UserSecrets_Id,
                        userSecretsId[badCharIndex],
                        badCharIndex));
            }

            var root = Environment.GetEnvironmentVariable("APPDATA") ??         // On Windows it goes to %APPDATA%\Microsoft\UserSecrets\
                        Environment.GetEnvironmentVariable("HOME");             // On Mac/Linux it goes to ~/.microsoft/usersecrets/

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPDATA")))
            {
                return Path.Combine(root, "Microsoft", "UserSecrets", userSecretsId, Secrets_File_Name);
            }
            else
            {
                return Path.Combine(root, ".microsoft", "usersecrets", userSecretsId, Secrets_File_Name);
            }
        }

        private static string GetUserSecretsIdFromFile(IFileProvider provider, string filename)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            var fileInfo = provider.GetFileInfo(filename);
            if (fileInfo == null || !fileInfo.Exists || string.IsNullOrEmpty(fileInfo.PhysicalPath))
            {
                var filePath = provider.GetFileInfo("/")?.PhysicalPath ?? "unknown";
                throw new FileNotFoundException(string.Format(Resources.Error_Missing_Identifer_File, filePath), filePath);
            }

            using (var stream = fileInfo.CreateReadStream())
            using (var streamReader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                var obj = JObject.Load(jsonReader);

                var userSecretsId = obj.Value<string>("userSecretsId");

                if (string.IsNullOrEmpty(userSecretsId))
                {
                    throw new InvalidOperationException(
                        string.Format(Resources.Error_Missing_UserSecretId_In_Json, fileInfo.PhysicalPath));
                }

                return userSecretsId;
            }
        }
    }
}