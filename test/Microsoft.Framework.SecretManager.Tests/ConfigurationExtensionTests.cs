// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Dnx.Runtime;
using Microsoft.Dnx.Runtime.Infrastructure;
using Microsoft.Framework.SecretManager;
using Microsoft.Framework.SecretManager.Tests;
using Xunit;

namespace Microsoft.Framework.Configuration.UserSecrets.Tests
{
    public class ConfigurationExtensionTests
    {
        private readonly IRuntimeEnvironment _runtimeEnv;

        public ConfigurationExtensionTests()
        {
            _runtimeEnv = ((IRuntimeEnvironment)CallContextServiceLocator.Locator.ServiceProvider.GetService(typeof(IRuntimeEnvironment)));
        }

        [Fact]
        public void AddUserSecrets_Does_Not_Fail_On_Non_Existing_File_Explicitly_Passed()
        {
            var builder = new ConfigurationBuilder()
                                .AddUserSecrets(userSecretsId: Guid.NewGuid().ToString());
        }

        [Fact]
        public void AddUserSecrets_Does_Not_Fail_On_Non_Existing_File()
        {
            var projectPath = UserSecretHelper.GetTempSecretProject();

            var builder = new ConfigurationBuilder(projectPath).AddUserSecrets();
            var configuration = builder.Build();
            Assert.Equal(null, configuration["Facebook:AppSecret"]);

            UserSecretHelper.DeleteTempSecretProject(projectPath);
        }

        [Fact]
        public void AddUserSecrets_With_An_Existing_Secret_File()
        {
            string userSecretsId;
            var projectPath = UserSecretHelper.GetTempSecretProject(out userSecretsId);

            var logger = new TestLogger(_runtimeEnv);
            var secretManager = new Program(_runtimeEnv) { Logger = logger };

            secretManager.Main(new string[] { "set", "Facebook:AppSecret", "value1", "-p", projectPath });

            var builder = new ConfigurationBuilder(projectPath).AddUserSecrets();
            var configuration = builder.Build();
            Assert.Equal("value1", configuration["Facebook:AppSecret"]);

            UserSecretHelper.DeleteTempSecretProject(projectPath);
        }

        [Fact]
        public void AddUserSecrets_With_SecretsId_Passed_Explicitly()
        {
            string userSecretsId;
            var projectPath = UserSecretHelper.GetTempSecretProject(out userSecretsId);

            var logger = new TestLogger(_runtimeEnv);
            var secretManager = new Program(_runtimeEnv) { Logger = logger };

            secretManager.Main(new string[] { "set", "Facebook:AppSecret", "value1", "-p", projectPath });

            var builder = new ConfigurationBuilder()
                                .AddUserSecrets(userSecretsId: userSecretsId);
            var configuration = builder.Build();

            Assert.Equal("value1", configuration["Facebook:AppSecret"]);
            UserSecretHelper.DeleteTempSecretProject(projectPath);
        }
    }
}