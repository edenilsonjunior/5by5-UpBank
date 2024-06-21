namespace Models.People
{
    public class Employee : Person
    {
        public bool Manager { get; set; }
        public int Registry { get; set; }
    }
}
