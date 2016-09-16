// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using Xunit;

namespace Microsoft.Extensions.Configuration.UserSecrets.Tests
{
    public class PathHelperTests
    {
        [Fact]
        public void Gives_Correct_Secret_Path()
        {
            string userSecretsId;
            var projectPath = UserSecretHelper.GetTempSecretProject(out userSecretsId);

            // intentionally test obsolete API to ensure behavior still works
            // TODO Remove in 2.0
            #pragma warning disable CS0618
            var actualSecretPath = PathHelper.GetSecretsPath(projectPath);
            #pragma warning restore CS0618

            var root = Environment.GetEnvironmentVariable("APPDATA") ??         // On Windows it goes to %APPDATA%\Microsoft\UserSecrets\
                        Environment.GetEnvironmentVariable("HOME");             // On Mac/Linux it goes to ~/.microsoft/usersecrets/

            var expectedSecretPath = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPDATA")) ?
                Path.Combine(root, "Microsoft", "UserSecrets", userSecretsId, "secrets.json") :
                Path.Combine(root, ".microsoft", "usersecrets", userSecretsId, "secrets.json");

            Assert.Equal(expectedSecretPath, actualSecretPath);

            UserSecretHelper.DeleteTempSecretProject(projectPath);
        }

        [Fact]
        public void Throws_If_Project_Json_Not_Found()
        {
            var projectPath = UserSecretHelper.GetTempSecretProject();
            File.Delete(Path.Combine(projectPath, "project.json"));

            Assert.Throws<FileNotFoundException>(() =>
            {
                PathHelper.GetSecretsPath(projectPath, "project.json");
            });

            UserSecretHelper.DeleteTempSecretProject(projectPath);
        }

        [Fact]
        public void Throws_If_Project_Json_Does_Not_Contain_UserSecretId()
        {
            var projectPath = UserSecretHelper.GetTempSecretProject();
            File.WriteAllText(Path.Combine(projectPath, "project.json"), "{}");

            Assert.Throws<InvalidOperationException>(() =>
            {
                PathHelper.GetSecretsPath(projectPath, "project.json");
            });

            UserSecretHelper.DeleteTempSecretProject(projectPath);
        }

        [Fact]
        public void Throws_If_UserSecretId_Contains_Invalid_Characters()
        {
            var projectPath = UserSecretHelper.GetTempSecretProject();

            foreach (var character in Path.GetInvalidPathChars())
            {
                UserSecretHelper.SetTempSecretInProject(projectPath, "Test" + character);
                Assert.Throws<InvalidOperationException>(() =>
                {
                    PathHelper.GetSecretsPath(projectPath, "project.json");
                });
            }

            UserSecretHelper.DeleteTempSecretProject(projectPath);
        }
    }
}