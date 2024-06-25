namespace Models.People
{
    public class Employee : Person
    {
        public static readonly string INSERT = @"INSERT INTO EmployeeDeleted (CPF, Name, BirthDt, Sex, AddressId, Salary, Phone, Email, Manager, Registry, AgencyNumber)
            VALUES (@CPF, @Name, @BirthDt, @Sex, @AddressId, @Salary, @Phone, @Email, @Manager, @Registry, @AgencyNumber);";

        public bool Manager { get; set; }
        public int Registry { get; set; }
    }
}
