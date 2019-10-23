//  Copyright (c) 2018 Demerzel Solutions Limited
//  This file is part of the Nethermind library.
// 
//  The Nethermind library is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  The Nethermind library is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public License
//  along with the Nethermind. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.Serialization;
using Nethermind.Core;
using Nethermind.Core.Json;
using Nethermind.Dirichlet.Numerics;
using Newtonsoft.Json;
using NUnit.Framework;
using Utf8Json;
using Utf8Json.Formatters;

namespace Nethermind.JsonRpc.Test
{
    public class Utf8JsonSerializerTests
    {
        private readonly IJsonSerializer _serializer = new Utf8EthereumJsonSerializer();

        [Test]
        public void Test1()
        {
            var json =
                "{\"jsonRpc\":\"2.0\",\"method\":\"eth_newFilter\",\"params\":[{\"topics\":[\"0x0000000000000000000000000000000000000000000000000000000012341234\"]}],\"id\":\"0xAf\"}";

            var rr = new JsonRpcRequest2
            {
                JsonRpc = "2.0",
                Method = "abc",
                Params = Array.Empty<object>(),
                Id = 5
            };

            var zz = _serializer.Serialize(rr);
            

//            var newto = _serializerNewtonsoft.DeserializeObjectOrArray<JsonRpcRequest2>(json);
            var req = _serializer.DeserializeObjectOrArray<JsonRpcRequest2>(json);

//            var des = _serializer.Deserialize<User>(s);
//            
//            var des2 = _serializer.Deserialize<User>(json);

        }
    }

    public class JsonRpcRequest2
    {
        public string JsonRpc { get; set; }
        public string Method { get; set; }
        public object[] Params { get; set; }
        public BigInteger? Id { get; set; }
    }
}