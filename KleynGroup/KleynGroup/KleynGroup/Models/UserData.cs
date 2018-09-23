using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using SQLite;

namespace KleynGroup.Models
{
    [Table("UserData")]
    public class UserData
    {
        [PrimaryKey, AutoIncrement, NotNull]
        public int Id { get; set; }
        //[Unique]
        [JsonProperty("remember_token")]
        public string remember_token { get; set; }
        [JsonProperty("username")] public string Username { get; set; }
        [JsonProperty("IsFrozen")] public int IsFrozen { get; set; }
        [JsonProperty("IsAdmin")] public int IsAdmin { get; set; }
        [JsonProperty("locationPermisson")] public int locationPermisson { get; set; }
        [JsonProperty("picturePermisson")] public int picturePermisson { get; set; }
        public string ErrorDescription { get; set; }
        public DateTime ExpireDate { get; set; }
        public double ExpireIn { get; set; }
    }
}
