using SQLite;
using System;

namespace CSS_Server.Models.Database.DBObjects
{
    /// <summary>
    /// Class which represents the Log table in the database.
    /// </summary>
    [Table("Logging")]
    public class DBLog : AbstractTable
    {
        [Column("level")]
        public int Level { get; set; }

        [Column("timestamp")]
        public DateTime TimeStamp { get; set; }

        [Column("message")]
        public string Message { get; set; }

    }
}
