using System.Text.Json.Serialization;
using Nethermind.Dirichlet.Numerics;

namespace Nethermind.EvmPlayground
{
    public class JsonRpcResponse
    {
        [JsonPropertyName("id")]
        public UInt256 Id { get; set; }
        
        [JsonPropertyName("jsonrpc")]
        public string Jsonrpc { get; set; }

        [JsonPropertyName("result")]
        public string Result { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; }
    }
    
    public class JsonRpcResponse<T>
    {
        [JsonPropertyName("id")]
        public UInt256 Id { get; set; }
        
        [JsonPropertyName("jsonrpc")]
        public string Jsonrpc { get; set; }
        
        [JsonPropertyName("result")]
        public T Result { get; set; }
        
        [JsonPropertyName("error")]
        public string Error { get; set; }
    }
}