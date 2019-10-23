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
using Nethermind.Core.Extensions;
using Nethermind.Dirichlet.Numerics;
using Utf8Json;

namespace Nethermind.Core.Json
{
    public class UInt256Formatter : IJsonFormatter<UInt256>
    {
        private readonly NumberConversion _conversion;

        public UInt256Formatter() : this(NumberConversion.Hex)
        {
        }

        public UInt256Formatter(NumberConversion conversion)
        {
            _conversion = conversion;
        }

        public void Serialize(ref JsonWriter writer, UInt256 value, IJsonFormatterResolver formatterResolver)
        {
            if (value.IsZero)
            {
                writer.WriteRaw(Extensions.Bytes0X0);
                return;
            }

            NumberConversion usedConversion = _conversion == NumberConversion.Decimal
                ? value < int.MaxValue ? NumberConversion.Decimal : NumberConversion.Hex
                : _conversion;

            switch (usedConversion)
            {
                case NumberConversion.PaddedHex:
                    writer.WriteRaw(Extensions.Bytes0X);
                    writer.WriteRaw(value.ToString("x64").TrimStart('0').GetUtf8Bytes());
                    break;
                case NumberConversion.Hex:
                    writer.WriteRaw(Extensions.Bytes0X);
                    writer.WriteRaw(value.ToString("x").TrimStart('0').GetUtf8Bytes());
                    break;
                case NumberConversion.Decimal:
                    writer.WriteInt32((int) value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public UInt256 Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            var (isString, isHex, segment) = reader.GetHexStringSegment();
            if (isString || isHex)
            {
                return ParseString(isHex, segment);
            }
            
            reader.AdvanceOffset(-segment.Count);
            
            return new UInt256(reader.ReadInt64());
        }
        
        private static UInt256 ParseString(bool isHex, ArraySegment<byte> utf8String)
        {
            if (!isHex)
            {
                return UInt256.Parse(System.Text.Encoding.UTF8.GetString(utf8String), NumberStyles.Integer);
            }

            return utf8String.IsHexZero()
                ? UInt256.Zero
                : UInt256.Parse(utf8String.ParseSegmentHexString(), NumberStyles.AllowHexSpecifier);
        }
    }
}