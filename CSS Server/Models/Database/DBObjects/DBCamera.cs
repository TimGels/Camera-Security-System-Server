using CSS_Server.Models.Database.Repositories;
using SQLite;

namespace CSS_Server.Models.Database
{
    /// <summary>
    /// Class which represents the Camera table in the database.
    /// </summary>
    [Repository(Name = "CameraRepository")]
    [Table("Camera")]
    public class DBCamera
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("passwd")]
        public string Password { get; set; }
    }
}
