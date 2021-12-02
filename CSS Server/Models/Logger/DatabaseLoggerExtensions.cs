using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;

namespace CSS_Server.Models.Logger
{
    public static class DatabaseLoggerExtensions
    {
        public static ILoggingBuilder AddDatabase(this ILoggingBuilder builder)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, DatabaseLoggerProvider>());
            
            return builder;
        }
    }
}
