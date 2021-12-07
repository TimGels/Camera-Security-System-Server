using SQLite;

namespace CSS_Server.Models.Database.DBObjects
{
    public abstract class AbstractTable
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
    }
}
