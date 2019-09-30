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

using System.Text.Json.Serialization;
using Nethermind.Core.Json;
using Nethermind.Core.Json.Converters;
using Nethermind.Dirichlet.Numerics;

namespace Nethermind.JsonRpc
{
    public class JsonRpcResponse
    {
        
        [JsonPropertyName("id")]
        [JsonConverter(typeof(DecimalUInt256Converter))]
        public UInt256 Id { get; set; }
        
        [JsonPropertyName("jsonrpc")]
        public string JsonRpc { get; set; }

        [JsonPropertyName("result")]
        public object Result { get; set; }

        [JsonPropertyName("error")]
        public Error Error { get; set; }
    }

    public class JsonRpcResponse<T>
    {
        [JsonPropertyName("id")]
        [JsonConverter(typeof(DecimalUInt256Converter))]
        public UInt256 Id { get; set; }
        
        [JsonPropertyName("jsonrpc")]
        public string JsonRpc { get; set; }

        [JsonPropertyName("result")]
        public T Result { get; set; }

        [JsonPropertyName("error")]
        public Error Error { get; set; }
    }
}