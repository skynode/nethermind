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

using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nethermind.Core.Json
{
    public class EthereumJsonSerializer : IJsonSerializer
    {
        public EthereumJsonSerializer()
        {
            foreach (var basicConverter in BasicConverters)
            {
                _readableSettings.Converters.Add(basicConverter);
            }
            
            foreach (var readableConverter in ReadableConverters)
            {
                _readableSettings.Converters.Add(readableConverter);
            }
        }

        public static IList<JsonConverter> BasicConverters { get; } = new List<JsonConverter>
        {
            new AddressConverter(),
            new KeccakConverter(),
            new BloomConverter(),
            new ByteArrayConverter(),
            new LongConverter(),
            new NullableLongConverter(),
            new UInt256Converter(),
            new NullableUInt256Converter(),
            new BigIntegerConverter(),
            new NullableBigIntegerConverter(),
            new PublicKeyConverter()
        };
        
        private static IList<JsonConverter> ReadableConverters { get; } = new List<JsonConverter>
        {
            new AddressConverter(),
            new KeccakConverter(),
            new BloomConverter(),
            new ByteArrayConverter(),
            new LongConverter(NumberConversion.Decimal),
            new NullableLongConverter(NumberConversion.Decimal),
            new UInt256Converter(NumberConversion.Decimal),
            new NullableUInt256Converter(NumberConversion.Decimal),
            new BigIntegerConverter(NumberConversion.Decimal),
            new NullableBigIntegerConverter(NumberConversion.Decimal),
            new PublicKeyConverter()
        };
        
        private readonly JsonSerializerOptions _basicSettings = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        private readonly JsonSerializerOptions _readableSettings = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, _basicSettings);
        }

        public (T Model, List<T> Collection) DeserializeObjectOrArray<T>(string json)
        {
            if (json.StartsWith("["))
            {
                return (default, JsonSerializer.Deserialize<List<T>>(json));
            }

            return (JsonSerializer.Deserialize<T>(json), default);
//            
//            using (var document = JsonDocument.Parse(json))
//            {
//                if (document.RootElement.ValueKind == JsonValueKind.Array)
//                {
//                    foreach (var element in document.RootElement.EnumerateArray())
//                    {
//                        UpdateParams(element);
//                    }
//
//                    return (default, JsonSerializer.Deserialize<List<T>>(document));
//                }
//
//                UpdateParams(document.RootElement);
//
//                return (JsonSerializer.Serialize(document), default);
//            }
        }

//        private void UpdateParams(JsonElement element)
//        {
//            JsonProperty paramsProperty;
//            foreach (var property in element.EnumerateObject())
//            {
//                if (property.Name != "params" && property.Name != "Params")
//                {
//                    continue;
//                }
//                
//                paramsProperty = property;
//                break;
//            }
//
//            if (string.IsNullOrWhiteSpace(paramsProperty.Name))
//            {
//                return;
//            }
//
//            var values = new List<string>();
//            foreach (var property in paramsProperty.Value.EnumerateArray())
//            {
//                var valueString = property.GetString();
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
            return JsonSerializer.Serialize(value, indented ? _readableSettings : _basicSettings);
        }

        public void RegisterConverter(JsonConverter converter)
        {
            _basicSettings.Converters.Add(converter);
            _readableSettings.Converters.Add(converter);
        }
    }
}