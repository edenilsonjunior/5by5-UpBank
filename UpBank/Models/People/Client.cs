using System.ComponentModel.DataAnnotations;

namespace Models.People
{
    
    public class Client : Person
    {
        [Key]
        public bool Restriction { get; set; }
    }
}
