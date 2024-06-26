using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class ClientDTOPost
    {
        public string Name { get; set; }
        [Key]
        public string CPF { get; set; }
        public DateTime BirthDt { get; set; }
        public char Sex { get; set; }
        public AddressDTO AddressDTO { get; set; }
        public double Salary { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int Registry { get; set; }
        [ForeignKey("AgencyNumber")]
        public string AgencyNumber { get; set; }
    }
}
