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
using System.Text.Json;
using System.Text.Json.Serialization;
using Nethermind.Core.Json;
using Nethermind.JsonRpc.Modules.Eth;
using Nethermind.JsonRpc.Modules.Trace;
using NUnit.Framework;

namespace Nethermind.JsonRpc.Test.Data
{
    public class SerializationTestBase
    {
        protected void TestConverter<T>(T item, Func<T, T, bool> equalityComparer, JsonConverter<T> converter)
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(converter);
            string result = JsonSerializer.Serialize(item, options);
            T deserialized = JsonSerializer.Deserialize<T>(result, options);

            Assert.True(equalityComparer(item, deserialized));
        }

        protected void TestSerialization<T>(T item, Func<T, T, bool> equalityComparer)
        {
            JsonSerializerOptions options = BuildSerializerOptions<T>();
            string result = JsonSerializer.Serialize(item,options);
            T deserialized = JsonSerializer.Deserialize<T>(result, options);

            Assert.True(equalityComparer(item, deserialized));
        }

        private static JsonSerializerOptions BuildSerializerOptions<T>()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            foreach (JsonConverter converter in EthModuleFactory.Converters)
            {
                options.Converters.Add(converter);
            }            
            
            foreach (JsonConverter converter in TraceModuleFactory.Converters)
            {
                options.Converters.Add(converter);
            }

            foreach (JsonConverter converter in EthereumJsonSerializer.BasicConverters)
            {
                options.Converters.Add(converter);
            }

            return options;
        }

        protected void TestOneWaySerialization<T>(T item, string expectedResult)
        {
            JsonSerializerOptions options = BuildSerializerOptions<T>();
            string result = JsonSerializer.Serialize(item,options);
            Assert.AreEqual(expectedResult, result, result.Replace("\"", "\\\""));
        }
    }
}