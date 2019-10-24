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
using Utf8Json;

namespace Nethermind.Core.Json
{
    public class ByteArrayFormatter : IJsonFormatter<byte[]>
    {
        public void Serialize(ref JsonWriter writer, byte[] value, IJsonFormatterResolver formatterResolver)
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }
            
            writer.WriteString(value.ToHexString(true));
        }

        public byte[] Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            
            var (isString, isHex, segment) = reader.GetHexStringSegment();
            if (isString || isHex)
            {
                return Bytes.FromHexString(System.Text.Encoding.UTF8.GetString(segment));
            }
            
            reader.AdvanceOffset(-segment.Count);

            return null;
        }
    }
}