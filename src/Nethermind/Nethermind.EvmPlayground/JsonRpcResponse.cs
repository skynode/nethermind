using System.Runtime.Serialization;
using Nethermind.Dirichlet.Numerics;

namespace Nethermind.EvmPlayground
{
    public class JsonRpcResponse
    {        
        [DataMember(Name= "id")]
        public UInt256 Id { get; set; }
        
        [DataMember(Name= "jsonrpc")]
        public string Jsonrpc { get; set; }

        [DataMember(Name= "result")]
        public string Result { get; set; }

        [DataMember(Name= "error")]
        public string Error { get; set; }
    }
    
    public class JsonRpcResponse<T>
    {
        [DataMember(Name= "id")]
        public UInt256 Id { get; set; }
        
        [DataMember(Name= "jsonrpc")]
        public string Jsonrpc { get; set; }
        
        [DataMember(Name= "result")]
        public T Result { get; set; }
        
        [DataMember(Name= "error")]
        public string Error { get; set; }

    }
}