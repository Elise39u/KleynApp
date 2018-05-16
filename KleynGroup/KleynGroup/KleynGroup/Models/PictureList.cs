using System;
using System.Collections.Generic;
using System.Text;

namespace KleynGroup.Models
{
        public class PictureInfo
        {
            public string label { get; set; }
            public string typecode { get; set; }
        }

        public class Records
        {
            public int offset { get; set; }
            public int pagesize { get; set; }
            public int recordcount { get; set; }
            public int totalrecords { get; set; }
            public List<PictureInfo> data { get; set; }
        }
}
