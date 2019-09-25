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

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nethermind.Core.Json
{
    public class UnforgivingJsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializerOptions _optionsIndented = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            WriteIndented = true
        };
        
        private readonly JsonSerializerOptions _optionsNotIndented = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            WriteIndented = false
        };

        public T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }

        public (T Model, List<T> Collection) DeserializeObjectOrArray<T>(string json)
        {
            if (json.StartsWith("["))
            {
                return (default, JsonSerializer.Deserialize<List<T>>(json));
            }

            return (JsonSerializer.Deserialize<T>(json), default);
            
//            var token = JToken.Parse(json);
//            if (token is JArray array)
//            {
//                foreach (var tokenElement in array)
//                {
//                    UpdateParams(tokenElement);
//                }
//
//                return (default, array.ToObject<List<T>>());
//            }
//            UpdateParams(token);
//
//            return (token.ToObject<T>(), null);
        }
        
//        private void UpdateParams(JToken token)
//        {
//            var paramsToken = token.SelectToken("params");
//            if (paramsToken == null)
//            {
//                paramsToken = token.SelectToken("Params");
//                if (paramsToken == null)
//                {
//                    return;
//                }
//
////                if (paramsToken == null)
////                {
////                    throw new FormatException("Missing 'params' token");
////                }
//            }
//            
//            var values = new List<string>();
//            foreach (var value in paramsToken.Value<IEnumerable<object>>())
//            {
//                var valueString = value?.ToString();
//                if (valueString == null)
//                {
//                    values.Add($"\"null\"");
//                    continue;
//                }
//                
//                if (valueString.StartsWith("{") || valueString.StartsWith("["))
//                {
//                    values.Add(Serialize(valueString));
//                    continue;
//                }
//                values.Add($"\"{valueString}\"");
//            }
//
//            var json = $"[{string.Join(",", values)}]";
//            paramsToken.Replace(JToken.Parse(json));
//        }

        public string Serialize<T>(T value, bool indented = false)
        {
            return JsonSerializer.Serialize(value, indented ? _optionsIndented : _optionsNotIndented);
        }

        public void RegisterConverter(JsonConverter converter)
        {
            throw new NotImplementedException();
        }
    }
}