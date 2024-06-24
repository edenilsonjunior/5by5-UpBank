using Models.Bank;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class EmployeeDTOEntity
    {
        public string Name { get; set; }
        [Key]
        public string CPF { get; set; }
        public DateTime BirthDt { get; set; }
        public char Sex { get; set; }
        [JsonIgnore]
        public string AddressId { get; set; }
        public double Salary { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool Manager { get; set; }
        public int Registry { get; set; }
        [ForeignKey("AgencyNumber")]
        public string AgencyNumber { get; set; }
    }
}
