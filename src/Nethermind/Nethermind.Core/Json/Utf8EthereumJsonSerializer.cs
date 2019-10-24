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
using Utf8Json;
    
namespace Nethermind.Core.Json
{
    public class Utf8EthereumJsonSerializer : IJsonSerializer
    {
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
            return JsonSerializer.Deserialize<T>(json, EthereumFormatterResolver.Instance);
        }

        public string Serialize<T>(T value, bool indented = false)
        {
            var bytes = JsonSerializer.Serialize(value,
                indented ? EthereumReadableFormatterResolver.Instance : EthereumFormatterResolver.Instance);
            
            return indented ? JsonSerializer.PrettyPrint(bytes) : System.Text.Encoding.UTF8.GetString(bytes);
        }

        public void RegisterFormatter(IJsonFormatter formatter)
        {
            EthereumFormatterResolver.AddFormatter(formatter);
            EthereumReadableFormatterResolver.AddFormatter(formatter);
        }
    }
}