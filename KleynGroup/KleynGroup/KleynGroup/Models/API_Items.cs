using System;
using System.Collections.Generic;
using System.Text;

namespace KleynGroup.Models
{
    public class Datum
    {
        public string label { get; set; }
        public string waarde { get; set; }
    }

    public class API_Items
    {
        public string itemnummer { get; set; }
        public string bussinesunit { get; set; }
        public string locatie { get; set; }
        public string voorraadstatus { get; set; }
        public List<Datum> data { get; set; }
    }

    public class Api_list
    {
        public List<API_Items> api_items { get; set; }
    }
}
