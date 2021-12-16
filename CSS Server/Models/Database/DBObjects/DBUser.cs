using SQLite;

namespace CSS_Server.Models.Database.DBObjects
{
    [Table("User")]
    public class DBUser : AbstractTable
    {
        [Unique]
        [Column("userName")]
        public string UserName { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("salt")]
        public string Salt { get; set; }
    }
}
