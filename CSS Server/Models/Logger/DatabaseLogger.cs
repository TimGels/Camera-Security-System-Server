using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using CSS_Server.Models.Database.DBObjects;
using CSS_Server.Models.Database.Repositories;

namespace CSS_Server.Models.Logger
{
    public class DatabaseLogger : ILogger
    {
        protected readonly DatabaseLoggerProvider _databaseLoggerProvider;
        private readonly Func<DatabaseLoggerConfiguration> _getConfig;
        private static readonly SQLiteRepository<DBLog> _repository = new SQLiteRepository<DBLog>();

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

            //add the log message to the database.
            Task.Run(() =>
            {
                //Create a new DBLog entry and insert it using the repository.
                _repository.Insert(new DBLog()
                {
                    Level = (int)logLevel,
                    Message = formatter(state, exception),
                    TimeStamp = DateTime.Now,
                });
            });

        }
    }
}
