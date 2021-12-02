using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CSS_Server.Models.Logger
{
    [ProviderAlias("DatabaseLogger")]
    public class DatabaseLoggerProvider : ILoggerProvider
    {
        private readonly DatabaseLoggerConfiguration _config;
        private readonly DatabaseLogger _databaseLogger;

        public DatabaseLoggerProvider(IOptionsMonitor<DatabaseLoggerConfiguration> config)
        {
            _config = config.CurrentValue;
            _databaseLogger = new DatabaseLogger(this, GetConfig);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _databaseLogger;
        }

        private DatabaseLoggerConfiguration GetConfig() => _config;

        public void Dispose()
        {

        }
    }
}
