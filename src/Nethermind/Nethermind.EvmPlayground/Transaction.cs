using System.Runtime.Serialization;

namespace Nethermind.EvmPlayground
{
    public class Transaction
    {
        public Transaction(byte[] sender, byte[] data)
        {
            From = sender.ToHexString(true);
            Data = data.ToHexString(true);
        }
       
        [DataMember(Name = "from")]
        public string From { get; }

        [DataMember(Name = "gas")]
        public string Gas { get; } = "0xF4240";

        [DataMember(Name = "gasPrice")]
        public string GasPrice { get; } = "0x4A817C800";

        [DataMember(Name = "to")]
        public string To { get; }

        [DataMember(Name = "data")]
        public string Data { get; }
    }
}