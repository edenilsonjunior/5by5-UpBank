using Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace Models.People
{

    public class Client : Person
    {
        public static readonly string InsertClientAccount = @"INSERT INTO ClientAccount VALUES (@AccountNumber, @ClientCPF)";

        public bool Restriction { get; set; }

        public bool Restriction { get; set; } 

        public Client() { }

        public Client(ClientDTO clientDTO)
        {
            CPF = clientDTO.CPF;
            Name = clientDTO.Name;
            BirthDt = clientDTO.BirthDt;
            Sex = clientDTO.Sex;
            Salary = clientDTO.Salary;
            Phone = clientDTO.Phone;
            Email = clientDTO.Email;
        }
    }
}
