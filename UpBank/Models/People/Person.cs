using System.ComponentModel.DataAnnotations;

namespace Models.People
{
    public abstract class Person
    {
        public string Name { get; set; }
        [Key]
        public string CPF { get; set; }
        public DateTime BirthDt { get; set; }
        public char Sex { get; set; }
        public Address Address { get; set; }
        public double Salary { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
