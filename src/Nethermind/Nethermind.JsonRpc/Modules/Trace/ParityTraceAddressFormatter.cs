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

using System.Collections.Generic;
using Utf8Json;

namespace Nethermind.JsonRpc.Modules.Trace
{
    public class ParityTraceAddressFormatter : IJsonFormatter<int[]>
    {
        public void Serialize(ref JsonWriter writer, int[] value, IJsonFormatterResolver formatterResolver)
        {
            if (value is null)
            {
                writer.WriteNull();
            }
            else
            {
                var counter = 0;
                writer.WriteBeginArray();
                foreach (int i in value)
                {
                    counter++;
                    writer.WriteInt32(i);
                    if (counter < value.Length)
                    {
                        writer.WriteValueSeparator();
                    }
                }

                writer.WriteEndArray();
            }
        }

        public int[] Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            List<int> result = new List<int>();
            JsonToken token;

            do
            {
                token = reader.GetCurrentJsonToken();
                if (token != JsonToken.Number)
                {
                    reader.AdvanceOffset(1);
                    continue;
                }

                result.Add(reader.ReadInt32());

            } while (!reader.ReadIsNull() && token != JsonToken.None);

            return result.ToArray();
        }
    }
}