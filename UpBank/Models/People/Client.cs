using Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace Models.People
{

    public class Client : Person
    {

        public bool Restriction { get; set; } //aqui adicionamos uma propriedade chamada Restrição bool, para indicar se o cliente tem restrição ou não

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
