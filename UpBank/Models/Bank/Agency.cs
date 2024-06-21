using Models.People;

namespace Models.Bank
{
    public class Agency
    {
        public string Number { get; set; }
        public Address Address { get; set; }
        public string CNPJ { get; set; }
        public List<Employee> Employees { get; set; }
        public bool Restriction { get; set; }
    }
}
