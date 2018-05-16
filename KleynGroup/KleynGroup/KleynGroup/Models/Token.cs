using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace KleynGroup.Models
{
    [Table("User")]
    public class Token
    {
        [PrimaryKey, AutoIncrement, NotNull]
        public int Id { get; set; }
        [Unique]
        public string access_token { get; set; }
        public string error_description { get; set; }
        public DateTime expire_date { get; set; }
        public double expire_in { get; set; }
    }
}
