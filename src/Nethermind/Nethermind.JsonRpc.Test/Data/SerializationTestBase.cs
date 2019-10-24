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
using System.Text;
using Nethermind.Core.Extensions;
using Nethermind.Core.Json;
using Nethermind.JsonRpc.Modules.Eth;
using Nethermind.JsonRpc.Modules.Trace;
using NUnit.Framework;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Nethermind.JsonRpc.Test.Data
{
    public class SerializationTestBase
    {
        protected void TestFormatter<T>(T item, Func<T, T, bool> equalityComparer, IJsonFormatter<T> formatter)
        {
            CompositeResolver.RegisterAndSetAsDefault(formatter);
            var resolver = CompositeResolver.Instance;
            JsonWriter writer = new JsonWriter();
            formatter.Serialize(ref writer, item, resolver);
            string result = Encoding.UTF8.GetString(writer.ToUtf8ByteArray());
            JsonReader reader = new JsonReader(result.GetUtf8Bytes());
            T deserialized = formatter.Deserialize(ref reader, resolver);

            Assert.True(equalityComparer(item, deserialized));
        }

        protected void TestSerialization<T>(T item, Func<T, T, bool> equalityComparer)
        {
            EthereumJsonSerializer serializer = BuildSerializer<T>();
            string result = serializer.Serialize(item);
            T deserialized = serializer.Deserialize<T>(result);

            Assert.True(equalityComparer(item, deserialized));
        }

        private static EthereumJsonSerializer BuildSerializer<T>()
        {
            EthereumJsonSerializer serializer = new EthereumJsonSerializer();
            foreach (var formatter in EthModuleFactory.Formatters)
            {
                serializer.RegisterFormatter(formatter);
            }            
            
            foreach (var converter in TraceModuleFactory.Formatters)
            {
                serializer.RegisterFormatter(converter);
            }

            return serializer;
        }

        protected void TestOneWaySerialization<T>(T item, string expectedResult)
        {
            EthereumJsonSerializer serializer = BuildSerializer<T>();
            string result = serializer.Serialize(item);
            Assert.AreEqual(expectedResult, result, result.Replace("\"", "\\\""));
        }
    }
}