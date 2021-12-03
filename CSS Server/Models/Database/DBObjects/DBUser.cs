﻿using SQLite;

namespace CSS_Server.Models.Database.DBObjects
{
    [Table("User")]
    public class DBUser
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Unique]
        [Column("email")]
        public string Email { get; set; }

        [Column("userName")]
        public string UserName { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("salt")]
        public string Salt { get; set; }
    }
}
