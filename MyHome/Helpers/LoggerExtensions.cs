using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace MyHome;

public static class LoggerExtensions
{
    public static void LogWithStack<T>(this ILogger<T> logger, string message, params object[] props)
    {
        var stack = new StackTrace(1);
        using var scope = logger.BeginScope(new Dictionary<string, object>()
        {
            {"stack_trace", stack.ToString()}
        });
        logger.LogInformation(message, props);
    }
}
