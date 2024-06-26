using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.People
{
    public class ClientCancelled
    {
        public string Name { get; set; }
        [Key]
        public string CPF { get; set; }
        public DateTime BirthDt { get; set; }
        public char Sex { get; set; }
        public string AddressId { get; set; }
        public double Salary { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool Restriction { get; set; }
    }
}
