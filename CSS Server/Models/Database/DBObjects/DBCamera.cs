using CSS_Server.Models.Database.DBObjects;
using SQLite;

namespace CSS_Server.Models.Database.DBObjects
{
    /// <summary>
    /// Class which represents the Camera table in the database.
    /// </summary>
    [Table("Camera")]
    public class DBCamera : AbstractTable
    {
        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("passwd")]
        public string Password { get; set; }

        [Column("salt")]
        public string Salt { get; set; }
    }
}
