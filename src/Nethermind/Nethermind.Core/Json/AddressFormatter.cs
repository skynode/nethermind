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

using Utf8Json;

namespace Nethermind.Core.Json
{
    public class AddressFormatter : IJsonFormatter<Address>
    {
        public void Serialize(ref JsonWriter writer, Address value, IJsonFormatterResolver formatterResolver)
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }
            
            writer.WriteString(value.ToString());
        }

        public Address Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            if (reader.ReadIsNull())
            {
                return null;
            }
            
            var (isString, isHex, segment) = reader.GetHexStringSegment();
            if (isString || isHex)
            {
                var address = System.Text.Encoding.UTF8.GetString(segment);
                return string.IsNullOrWhiteSpace(address) ? null : new Address(address);
            }
            
            reader.AdvanceOffset(-segment.Count);

            return null;
        }
    }
}