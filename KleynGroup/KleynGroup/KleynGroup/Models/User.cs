using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace KleynGroup
{

    [Table("User")]
    public class User
    {   
        [PrimaryKey, AutoIncrement, NotNull]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

}