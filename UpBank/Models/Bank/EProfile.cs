using System.Text.Json.Serialization;

namespace Models.Bank
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EProfile
    {
        Academic,
        Normal,
        VIP
    }
}
