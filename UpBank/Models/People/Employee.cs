using System.ComponentModel.DataAnnotations;

namespace Models.People
{
    public class Employee : Person
    {
        public bool Manager { get; set; }
        [Key]
        public int Registry { get; set; }
    }
}
