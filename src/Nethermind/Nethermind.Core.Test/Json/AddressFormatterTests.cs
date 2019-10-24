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

using Nethermind.Core.Extensions;
using Nethermind.Core.Json;
using Nethermind.Core.Test.Builders;
using NUnit.Framework;
using Utf8Json;

namespace Nethermind.Core.Test.Json
{
    [TestFixture]
    public class AddressFormatterTests
    {
        [Test]
        public void Should_deserialize()
        {
            AddressFormatter formatter = new AddressFormatter();
            JsonReader reader = new JsonReader("".GetUtf8Bytes());
            Address result = formatter.Deserialize(ref reader, EthereumFormatterResolver.Instance);
            Assert.AreEqual(null, result);
        }
        
        [Test]
        public void Should_serialize()
        {
            AddressFormatter formatter = new AddressFormatter();
            JsonWriter writer = new JsonWriter();
            formatter.Serialize(ref writer, TestItem.AddressA,  EthereumFormatterResolver.Instance);
            var bytes = writer.ToUtf8ByteArray();
            Assert.AreEqual(System.Text.Encoding.UTF8.GetString(bytes), TestItem.AddressA.ToString());
        }
    }
}