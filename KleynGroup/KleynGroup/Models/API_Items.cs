using System.Collections.Generic;

namespace KleynGroup.Models
{
    public class Datum
    {
        public string Label { get; set; }
        public string Waarde { get; set; }
    }

    public class ApiItems
    {
        public string Itemnummer { get; set; }
        public string Bussinesunit { get; set; }
        public string Locatie { get; set; }
        public string Voorraadstatus { get; set; }
        public List<Datum> Data { get; set; }
    }

    public class ApiList
    {
        public List<ApiItems> ApiItems { get; set; }
    }
}