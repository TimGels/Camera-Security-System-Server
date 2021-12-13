using SQLite;

namespace CSS_Server.Models.Database.DBObjects
{
    /// <summary>
    /// Class which represents the Log table in the database.
    /// </summary>
    [Table("Logging")]
    public class DBLog
    {
        [Column("level")]
        public int Level { get; set; }

        [Column("message")]
        public string Message { get; set; }

    }
}
