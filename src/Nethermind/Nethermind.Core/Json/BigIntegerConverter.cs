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
using System.Globalization;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nethermind.Core.Json
{
    public class BigIntegerConverter : JsonConverter<BigInteger>
    {
        private readonly NumberConversion _conversion;

        public BigIntegerConverter()
            : this(NumberConversion.Hex)
        {
        }

        public BigIntegerConverter(NumberConversion conversion)
        {
            _conversion = conversion;
        }

        public override BigInteger Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TryGetInt64(out var value))
            {
                return value;
            }
            
            string s = reader.GetString();
            if (!s.StartsWith("0x"))
            {
                return BigInteger.Parse(s);
            }
            
            if (s == "0x0")
            {
                return BigInteger.Zero;
            }

            if (s.StartsWith("0x0"))
            {
                return BigInteger.Parse(s.AsSpan(2), NumberStyles.AllowHexSpecifier);
            }

            if (s.StartsWith("0x"))
            {
                Span<char> withZero = new Span<char>(new char[s.Length - 1]);
                withZero[0] = '0';
                s.AsSpan(2).CopyTo(withZero.Slice(1));
                return BigInteger.Parse(withZero, NumberStyles.AllowHexSpecifier);
            }

            return BigInteger.Parse(s, NumberStyles.Integer);
        }

        public override void Write(Utf8JsonWriter writer, BigInteger value, JsonSerializerOptions options)
        {
            if (value.IsZero)
            {
                writer.WriteStringValue("0x0");
                return;
            }

            switch (_conversion)
            {
                case NumberConversion.PaddedHex:
                    writer.WriteStringValue($"\"0x{value.ToString("x64").TrimStart('0')}\"");
                    break;
                case NumberConversion.Hex:
                    writer.WriteStringValue($"\"0x{value.ToString("x").TrimStart('0')}\"");
                    break;
                case NumberConversion.Decimal:
                    writer.WriteStringValue(value.ToString());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}