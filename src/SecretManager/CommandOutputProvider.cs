// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Framework.Logging;
using Microsoft.Framework.Runtime;

namespace SecretManager
{
    public class CommandOutputProvider : ILoggerProvider
    {
        private readonly bool _isWindows;

        public CommandOutputProvider(IRuntimeEnvironment runtimeEnv)
        {
            _isWindows = runtimeEnv.OperatingSystem == "Windows";
        }

        public ILogger CreateLogger(string name)
        {
            return new CommandOutputLogger(this, useConsoleColor: _isWindows);
        }

        public LogLevel LogLevel { get; set; } = LogLevel.Information;
    }
}