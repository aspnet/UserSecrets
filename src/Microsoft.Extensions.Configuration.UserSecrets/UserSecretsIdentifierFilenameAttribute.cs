// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reflection;

namespace Microsoft.Extensions.Configuration.UserSecrets
{
    /// <summary>
    ///     <para>
    ///     Represents the filename that <see cref="ConfigurationExtensions.AddUserSecrets(IConfigurationBuilder, Assembly)" /> will load to find the user secret identifier.
    ///     </para>
    ///     <para>
    ///     The identifer file should be a JSON file with a top-level key 'userSecretsId' with string value. The value identifies the configuration source of
    ///     user secrets that configuration should load.
    ///     </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = false)]
    public class UserSecretsIdentifierFileNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes an instance of <see cref="UserSecretsIdentifierFileNameAttribute" />
        /// </summary>
        /// <param name="fileName">The filename (relative path)</param>
        public UserSecretsIdentifierFileNameAttribute(string fileName)
        {
            FileName = fileName;
        }

        /// <summary>
        /// The filename of the JSON file that stores the user secret id.
        /// <remarks>
        /// This can be null or empty. Callers should check the value before using.
        /// </remarks>
        /// </summary>
        /// <returns>The file name</returns>
        public string FileName { get; set; }
    }
}