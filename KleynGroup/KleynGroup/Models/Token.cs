using System;
using Newtonsoft.Json;
using SQLite;

namespace KleynGroup.Models
{
    [Table("UserData")]
    public class Token
    {
        [PrimaryKey, AutoIncrement, NotNull]
        public int Id { get; set; }
        [Unique]
        [JsonProperty("access_token")] public string AccessToken { get; set; }
        [JsonProperty("username")] public string Username { get; set; }
        [JsonProperty("IsFrozen")] public string IsFrozen { get; set; }
        [JsonProperty("IsAdmin")] public string IsAdmin { get; set; }
        [JsonProperty("ael")] public string Ael { get; set; }
        [JsonProperty("aes")] public string Aes { get; set; }
        [JsonProperty("aeb")] public string Aeb { get; set; }
        [JsonProperty("aev")] public string Aev { get; set; }
        [JsonProperty("aep")] public string Aep { get; set; }
        public string ErrorDescription { get; set; }
        public DateTime ExpireDate { get; set; }
        public double ExpireIn { get; set; }
    }

}

