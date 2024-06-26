using Models.People;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class EmployeeDTO
    {
        public string Name { get; set; }
        public string CPF { get; set; }
        public DateTime BirthDt { get; set; }
        public char Sex { get; set; }
        public AddressDTO Address { get; set; } //IdAddress
        public double Salary { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool Manager { get; set; }
        public int Registry { get; set; }
    }
}
