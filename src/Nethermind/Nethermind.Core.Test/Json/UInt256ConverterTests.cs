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

using System.IO;
using System.Text.Json;
using Nethermind.Core.Json;
using Nethermind.Core.Json.Converters;
using Nethermind.Dirichlet.Numerics;
using NUnit.Framework;

namespace Nethermind.Core.Test.Json
{
    [TestFixture]
    public class UInt256ConverterTests
    {
        [Test]
        public void Regression_0xa00000()
        {
            UInt256Converter converter = new UInt256Converter();
            Utf8JsonReader reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes("0xa00000"));
            reader.Read();
            UInt256 result = converter.Read(ref reader, typeof(UInt256), null);
            Assert.AreEqual(UInt256.Parse("10485760"), result);
        }
        
        [Test]
        public void Can_read_0x0()
        {
            UInt256Converter converter = new UInt256Converter();
            Utf8JsonReader reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes("0x0"));
            reader.Read();
            UInt256 result = converter.Read(ref reader, typeof(UInt256), null);
            Assert.AreEqual(UInt256.Parse("0"), result);
        }
        
        [Test]
        public void Can_read_0()
        {
            UInt256Converter converter = new UInt256Converter();
            Utf8JsonReader reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes("0"));
            reader.Read();
            UInt256 result = converter.Read(ref reader, typeof(UInt256), null);
            Assert.AreEqual(UInt256.Parse("0"), result);
        }
        
        [Test]
        public void Can_read_1()
        {
            UInt256Converter converter = new UInt256Converter();
            Utf8JsonReader reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes("1"));
            reader.Read();
            UInt256 result = converter.Read(ref reader, typeof(UInt256), null);
            Assert.AreEqual(UInt256.Parse("1"), result);
        }
    }
}