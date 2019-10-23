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
using Nethermind.Dirichlet.Numerics;
using NUnit.Framework;
using Utf8Json;

namespace Nethermind.Core.Test.Json
{
    [TestFixture]
    public class UInt256FormatterTests
    {
        [TestCase("0xa00000", "10485760")]
        [TestCase("\"0xa00000\"", "10485760")]
        [TestCase("0x", "0")]
        [TestCase("0x0", "0")]
        [TestCase("0", "0")]
        [TestCase("1", "1")]
        [TestCase("\"1\"", "1")]
        [TestCase("0x20000", "131072")]
        public void Should_deserialize(string value, string expected)
        {
            UInt256Formatter formatter = new UInt256Formatter();
            JsonReader reader = new JsonReader(value.GetUtf8Bytes());
            UInt256 result = formatter.Deserialize(ref reader, EthereumFormatterResolver.Instance);
            Assert.AreEqual(UInt256.Parse(expected), result);
        }
    }
}