using Models.DTO;
using Models.People;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Models.Bank
{
    public class Agency
    {
        [Key]
        public string Number { get; set; }
        [NotMapped]
        public Address Address { get; set; }
        [JsonIgnore]
        public string AddressId { get; set; }
        public string CNPJ { get; set; }
        public virtual List<EmployeeDTOEntity> Employees { get; set; }
        public bool Restriction { get; set; }
    }
}
