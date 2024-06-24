using System.Text.Json.Serialization;

namespace Models.Bank
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ETransactionType
    {
        Withdraw,   // saque
        Deposit,    // depósito
        Lending,    // empréstimo
        Payment,    // pagamento
        Transfer    // transferência
    }
}
