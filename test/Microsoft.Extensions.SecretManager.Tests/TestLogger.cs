// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.SecretManager.Tests
{
    public class TestLogger : ILogger
    {
        private readonly CommandOutputLogger _commandOutputLogger;

        public TestLogger(IRuntimeEnvironment runtimeEnv, bool debug = false)
        {
            var commandOutputProvider = new CommandOutputProvider(runtimeEnv);
            if (debug)
            {
                commandOutputProvider.LogLevel = LogLevel.Debug;
            }

            _commandOutputLogger = (CommandOutputLogger)commandOutputProvider.CreateLogger("");
        }

        public List<string> Messages { get; set; } = new List<string>();

        public IDisposable BeginScopeImpl(object state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _commandOutputLogger.IsEnabled(logLevel);
        }

        public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                Messages.Add(formatter(state, exception));
            }
        }
    }
}