using System;
using System.Collections.Generic;
using System.Text;

namespace KleynGroup.Models
{
    public class Idlist
    {
        public string Id { get; set; }
    }

    public class Youtube
    {
        public int offset { get; set; }
        public int pagesize { get; set; }
        public int recordcount { get; set; }
        public int totalrecords { get; set; }
        public List<Idlist> Data { get; set; }
    }
}
