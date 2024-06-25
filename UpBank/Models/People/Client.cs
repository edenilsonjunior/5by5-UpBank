using System.ComponentModel.DataAnnotations;

namespace Models.People
{
    
    public class Client : Person
    {
        
        public bool Restriction { get; set; } //aqui adicionamos uma propriedade chamada Restrição bool, para indicar se o cliente tem restrição ou não
        public DateTime DateOfBirth { get; set; }
    }
}
