using System;

namespace KleynGroup.Models
{
    public class Locatieitem
    {
        public int Itemnummer { get; set; }
        public string Locatie { get; set; }
        public DateTime Tijdstip { get; set; }
        public double Lengtegraad { get; set; }
        public double Breedtegraad { get; set; }
        public string Gebruiker { get; set; }
        public string Errorcode { get; set; }

        private object _result;

        public object GetResult()
        {
            return _result;
        }

        public void SetResult(object value)
        {
            _result = value;
        }
    }
}
