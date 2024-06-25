using Models.DTO;
using Models.People;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Models.Bank
{
    public class Agency
    {
        public static readonly string INSERT = "INSERT INTO AgencyDeleted (Number, AddressId, CNPJ, Restriction) VALUES (@Number, @AddressId, @CNPJ, @Restriction);";

        public string Number { get; set; }
        public Address Address { get; set; }
        public string CNPJ { get; set; }
        public List<Employee> Employees { get; set; }
        public bool Restriction { get; set; }
    }
}
