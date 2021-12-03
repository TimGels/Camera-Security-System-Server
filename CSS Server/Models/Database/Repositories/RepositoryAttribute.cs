using System;

namespace CSS_Server.Models.Database.Repositories
{
    [AttributeUsage(AttributeTargets.All)]
    public class RepositoryAttribute : Attribute
    {
        private string _name;

        public virtual string Name
        {
            get
            {
                return this.GetType().Namespace + "." + _name;
            }
            set
            {
                _name = value;
            }
        }
    }
}
