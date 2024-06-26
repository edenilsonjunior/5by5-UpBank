using Models.People;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class AgencyDTOEntity
    {
        [Key]
        public string Number { get; set; }
        [NotMapped]
        public Address Address { get; set; }
        public string AddressId { get; set; }
        public string CNPJ { get; set; }
        public virtual List<EmployeeDTOEntity> Employees { get; set; }
        public bool Restriction { get; set; }
    }
}
