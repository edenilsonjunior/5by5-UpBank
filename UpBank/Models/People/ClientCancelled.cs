using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.People
{
    public class ClientCancelled : Person
    {
        public bool Restriction { get; set; } //aqui adicionamos uma propriedade chamada Restrição bool, para indicar se o cliente tem restrição ou não
        public DateTime DateOfBirth { get; set; }
    }
}
