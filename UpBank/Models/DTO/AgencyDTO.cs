using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class AgencyDTO
    {
        public string? AddressId { get; set; }
        public string? CNPJ { get; set; }
        public virtual List<int>? Employees { get; set; }
        public bool? Restriction { get; set; }
    }
}
