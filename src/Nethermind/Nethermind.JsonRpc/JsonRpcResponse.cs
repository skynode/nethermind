/*
 * Copyright (c) 2018 Demerzel Solutions Limited
 * This file is part of the Nethermind library.
 *
 * The Nethermind library is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * The Nethermind library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with the Nethermind. If not, see <http://www.gnu.org/licenses/>.
 */

using System.Runtime.Serialization;
using Nethermind.Core.Json;
using Nethermind.Dirichlet.Numerics;
using Utf8Json;

namespace Nethermind.JsonRpc
{
    public class JsonRpcResponse
    {
        [JsonFormatter(typeof(UInt256Formatter), NumberConversion.Decimal)]
        [DataMember(Name = "id")]
        public UInt256 Id { get; set; }
        
        [DataMember(Name = "jsonrpc")]
        public string JsonRpc { get; set; }

        [DataMember(Name = "result")]
        public object Result { get; set; }
    }
    
    public class JsonRpcErrorResponse : JsonRpcResponse
    {
        [DataMember(Name = "result")]
        public new object Result { get; set; }
        
        [DataMember(Name = "error")]
        public Error Error { get; set; }
    }

    public class JsonRpcResponse<T>
    {
        [JsonFormatter(typeof(UInt256Formatter), NumberConversion.Decimal)]
        [DataMember(Name = "id", Order = 0)]
        public UInt256 Id { get; set; }
        
        [DataMember(Name = "jsonrpc", Order = 1)]
        public string JsonRpc { get; set; }

        [DataMember(Name = "result")]
        public T Result { get; set; }

        [DataMember(Name = "error")]
        public Error Error { get; set; }
    }
}