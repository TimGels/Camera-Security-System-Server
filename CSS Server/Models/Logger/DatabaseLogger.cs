using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;

namespace CSS_Server.Models.Logger
{
    public class DatabaseLogger : ILogger
    {
        protected readonly DatabaseLoggerProvider _databaseLoggerProvider;
        private readonly Func<DatabaseLoggerConfiguration> _getConfig;

        public DatabaseLogger([NotNull]DatabaseLoggerProvider databaseLoggerProvider, Func<DatabaseLoggerConfiguration> getConfig)
        {
            _databaseLoggerProvider = databaseLoggerProvider;
            _getConfig = getConfig;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel) || state == null || eventId.Id != _getConfig().EventId)
                return;

            Console.WriteLine("lvl: " + logLevel + " eventid: " + eventId.Id + " Database logger! Heck yea! " + formatter(state, exception));
        }
    }
}
