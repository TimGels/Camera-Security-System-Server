using CSS_Server.Models.Database.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

namespace CSS_Server.Models.Database
{
    /// <summary>
    /// Provides an easy way to get access to repositories.
    /// </summary>
    public class RepositoryProvider
    {
        private readonly ILogger<RepositoryProvider> _logger;
        public RepositoryProvider(ILogger<RepositoryProvider> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets the repository of an entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public RepositoryInterface GetRepository<T>()
        {
            return (RepositoryInterface)CreateRepository(GetRepositoryName<T>());
        }

        /// <summary>
        /// Retrieves the Repository name of the given Entity.
        /// </summary>
        /// <typeparam name="T">The Entity type</typeparam>
        /// <returns></returns>
        private string GetRepositoryName<T>()
        {
            var dnAttribute = typeof(T).GetCustomAttribute<RepositoryAttribute>(true);
            if (dnAttribute != null)
            {
                return dnAttribute.Name;
            }
            return null;
        }

        /// <summary>
        /// Creates a repository based on the fully qualified name of class.
        /// </summary>
        /// <param name="fullyQualifiedName">Namespace + class name</param>
        /// <returns></returns>
        private object CreateRepository(string fullyQualifiedName)
        {
            try
            {
                return Activator.CreateInstance(Type.GetType(fullyQualifiedName));
            }
            catch (TypeLoadException exception)
            {
                _logger.LogCritical("ERROR: {exception.Message}", exception.Message);
                throw;
            }
            catch (ArgumentNullException exception)
            {
                _logger.LogCritical("ERROR: {exception.Message}", exception.Message);
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogCritical("ERROR: {exception.Message}", exception.Message);
                throw;
            }
        }
    }
}
