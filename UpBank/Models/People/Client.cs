namespace Models.People
{
    public class Client : Person
    {
        public static readonly string InsertClientAccount = @"INSERT INTO ClientAccount VALUES (@AccountNumber, @ClientCPF)";

        public bool Restriction { get; set; }
    }
}
