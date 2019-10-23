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

using System.Numerics;
using Nethermind.Core.Extensions;
using Nethermind.Core.Json;
using NUnit.Framework;
using Utf8Json;

namespace Nethermind.Core.Test.Json
{
    [TestFixture]
    public class LongFormatterTests
    {
        [TestCase("0xa00000", 10485760)]
        [TestCase("\"0xa00000\"", 10485760)]
        [TestCase("0x", 0)]
        [TestCase("0x0", 0)]
        [TestCase("0", 0)]
        [TestCase("1", 1)]
        [TestCase("\"1\"", 1)]
        [TestCase("0x20000", 131072)]
        [TestCase("0x118c30", 1150000)]
        public void Should_deserialize(string value, long expected)
        {
            LongFormatter formatter = new LongFormatter();
            JsonReader reader = new JsonReader(value.GetUtf8Bytes());
            long result = formatter.Deserialize(ref reader, EthereumFormatterResolver.Instance);
            Assert.AreEqual(expected, result);
        }
        
        [TestCase(0, "0x0")]
        [TestCase(1, "0x1")]
        [TestCase(10485760, "0xa00000")]
        [TestCase(131072, "0x20000")]
        [TestCase(1150000, "0x118c30")]
        public void Should_serialize(long value, string expected)
        {
            LongFormatter formatter = new LongFormatter();
            JsonWriter writer = new JsonWriter();
            formatter.Serialize(ref writer, value,  EthereumFormatterResolver.Instance);
            var bytes = writer.ToUtf8ByteArray();
            Assert.AreEqual(System.Text.Encoding.UTF8.GetString(bytes), expected);
        }
    }
}