// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reflection;

namespace Microsoft.Extensions.Configuration.UserSecrets.Internal
{
    internal static class AssemblyExtensions
    {
        // Set to project.json for maximum compatibility with 1.0.0
        private const string DefaultIdentifierFileName = "project.json";

        public static string GetUserSecretsFileNameOrDefault(this Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            return assembly.GetCustomAttribute<UserSecretsIdentifierFileNameAttribute>()?.FileName
                ?? DefaultIdentifierFileName;
        }
    }
}