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
using Utf8Json;

namespace Nethermind.Core.Json
{
    public class LongFormatter : IJsonFormatter<long>, IObjectPropertyNameFormatter<long>
    {
        private readonly NumberConversion _conversion;

        public LongFormatter() : this(NumberConversion.Hex)
        {
        }

        public LongFormatter(NumberConversion conversion)
        {
            _conversion = conversion;
        }

        public void Serialize(ref JsonWriter writer, long value, IJsonFormatterResolver formatterResolver)
        {
            if (value == 0L)
            {
                writer.WriteString("0x0");
                return;
            }

            switch (_conversion)
            {
                case NumberConversion.PaddedHex:
                    writer.WriteString(string.Concat("0x", value.ToString("x64").TrimStart('0')));
                    break;
                case NumberConversion.Hex:
                    writer.WriteString(string.Concat("0x", value.ToString("x").TrimStart('0')));
                    break;
                case NumberConversion.Decimal:
                    writer.WriteString(value.ToString());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public long Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var (isString, isHex, segment) = reader.GetHexStringSegment();
            if (isString || isHex)
            {
                return ParseString(isHex, segment);
            }
            
            reader.AdvanceOffset(-segment.Count);

            return reader.ReadInt64();
        }

        private static long ParseString(bool isHex, ArraySegment<byte> utf8String)
        {
            if (!isHex)
            {
                return long.Parse(System.Text.Encoding.UTF8.GetString(utf8String), NumberStyles.Integer);
            }

            return utf8String.IsHexZero()
                ? 0L
                : long.Parse(utf8String.ParseSegmentHexString(), NumberStyles.AllowHexSpecifier);
        }
        
        public static long FromString(string s)
        {
            if (s == "0x0")
            {
                return 0L;
            }

            if (s.StartsWith("0x0"))
            {
                return long.Parse(s.AsSpan(2), NumberStyles.AllowHexSpecifier);
            }

            if (s.StartsWith("0x"))
            {
                Span<char> withZero = new Span<char>(new char[s.Length - 1]);
                withZero[0] = '0';
                s.AsSpan(2).CopyTo(withZero.Slice(1));
                return long.Parse(withZero, NumberStyles.AllowHexSpecifier);
            }

            return long.Parse(s, NumberStyles.Integer);
        }

        public void SerializeToPropertyName(ref JsonWriter writer, long value, IJsonFormatterResolver formatterResolver)
        {
            Serialize(ref writer, value, formatterResolver);
        }

        public long DeserializeFromPropertyName(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            return Deserialize(ref reader, formatterResolver);
        }
    }
}