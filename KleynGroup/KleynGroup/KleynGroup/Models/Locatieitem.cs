using System;
using System.Collections.Generic;
using System.Text;

namespace KleynGroup.Models
{
    public class Locatieitem
    {
        public int itemnummer { get; set; }
        public string locatie { get; set; }
        public DateTime tijdstip { get; set; }
        public double lengtegraad { get; set; }
        public double breedtegraad { get; set; }
        public string gebruiker { get; set; }
        public string errorcode { get; set; }
    }
}
