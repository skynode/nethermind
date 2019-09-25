using System.Text.Json.Serialization;

namespace Nethermind.EvmPlayground
{
    public class Transaction
    {
        public Transaction(byte[] sender, byte[] data)
        {
            From = sender.ToHexString(true);
            Data = data.ToHexString(true);
        }
       
        [JsonPropertyName("from")]
        public string From { get; }

        [JsonPropertyName("gas")]
        public string Gas { get; } = "0xF4240";

        [JsonPropertyName("gasPrice")]
        public string GasPrice { get; } = "0x4A817C800";

        [JsonPropertyName("to")]
        public string To { get; }

        [JsonPropertyName("data")]
        public string Data { get; }
    }
}