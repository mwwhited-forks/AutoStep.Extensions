﻿using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace AutoStep.Extensions.Tests.Shared
{
    internal static class TestLogFactory
    {
        public static ILoggerFactory Create(ITestOutputHelper outputHelper)
        {
            return LoggerFactory.Create(cfg =>
            {
                cfg.SetMinimumLevel(LogLevel.Debug);
                cfg.AddProvider(new TestLogProvider(outputHelper));
            });
        }

        public static ILoggerFactory CreateNull()
        {
            return new LoggerFactory();
        }
    }
}
