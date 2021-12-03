using CSS_Server.Models.Database.Repositories;
using System;
using System.Reflection;

namespace CSS_Server.Models.Database
{
    /// <summary>
    /// Provides an easy way to get access to repositories.
    /// </summary>
    public static class RepositoryProvider
    {
        /// <summary>
        /// Gets the repository of an entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static RepositoryInterface GetRepository<T>()
        {
            return (RepositoryInterface)CreateRepository(GetRepositoryName<T>());
        }

        /// <summary>
        /// Retrieves the Repository name of the given Entity.
        /// </summary>
        /// <typeparam name="T">The Entity type</typeparam>
        /// <returns></returns>
        private static string GetRepositoryName<T>()
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
        private static object CreateRepository(string fullyQualifiedName)
        {
            Type t = Type.GetType(fullyQualifiedName);
            return Activator.CreateInstance(t);
        }
    }
}
