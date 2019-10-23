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

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nethermind.Core.Json
{
    public class Utf8EthereumJsonSerializer : IJsonSerializer
    {
        public T DeserializeAnonymousType<T>(string json, T definition)
        {
            throw new System.NotImplementedException();
        }

        public (T Model, List<T> Collection) DeserializeObjectOrArray<T>(string json)
        {
            if (json.StartsWith("["))
            {
                return (default, Deserialize<List<T>>(json));
            }

            return (Deserialize<T>(json), null);
        }

        public T Deserialize<T>(string json)
        {
            return Utf8Json.JsonSerializer.Deserialize<T>(json, EthereumFormatterResolver.Instance);
        }

        public string Serialize<T>(T value, bool indented = false)
        {
            var bytes = Utf8Json.JsonSerializer.Serialize(value, EthereumFormatterResolver.Instance);
            return indented ? Utf8Json.JsonSerializer.PrettyPrint(bytes) : System.Text.Encoding.UTF8.GetString(bytes);
        }

        public void RegisterConverter(JsonConverter converter)
        {
            throw new System.NotImplementedException();
        }
    }
}