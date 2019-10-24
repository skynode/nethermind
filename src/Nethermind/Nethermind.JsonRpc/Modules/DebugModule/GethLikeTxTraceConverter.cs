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
using System.Collections.Generic;
using Nethermind.Evm.Tracing;
using Nethermind.JsonRpc.Modules.Trace;
using Utf8Json;

namespace Nethermind.JsonRpc.Modules.DebugModule
{
    public class GethLikeTxTraceConverter : IJsonFormatter<GethLikeTxTrace>
    {
        public void Serialize(ref Utf8Json.JsonWriter writer, GethLikeTxTrace value,
            IJsonFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteBeginObject();

            writer.WriteProperty("gas", value.Gas, formatterResolver);
            writer.WriteProperty("failed", value.Failed, formatterResolver);
            writer.WriteProperty("returnValue", value.ReturnValue, formatterResolver);

            writer.WritePropertyName("structLogs");
            WriteEntries(writer, value.Entries, formatterResolver);

            writer.WriteEndObject();
        }

        private static void WriteEntries(JsonWriter writer, List<GethTxTraceEntry> entries,
            IJsonFormatterResolver formatterResolver)
        {
            writer.WriteBeginArray();
            foreach (GethTxTraceEntry entry in entries)
            {
                writer.WriteBeginObject();
                writer.WriteProperty("pc", entry.Pc, formatterResolver);
                writer.WriteProperty("op", entry.Operation, formatterResolver);
                writer.WriteProperty("gas", entry.Gas, formatterResolver);
                writer.WriteProperty("gasCost", entry.GasCost, formatterResolver);
                writer.WriteProperty("depth", entry.Depth, formatterResolver);
                writer.WriteProperty("error", entry.Error, formatterResolver);
                writer.WritePropertyName("stack");
                writer.WriteBeginArray();
                foreach (string stackItem in entry.Stack)
                {
                    writer.WriteString(stackItem);
                }

                writer.WriteEndArray();

                writer.WritePropertyName("memory");
                writer.WriteBeginArray();
                foreach (string memory in entry.Memory)
                {
                    writer.WriteString(memory);
                }

                writer.WriteEndArray();

                writer.WritePropertyName("storage");
                writer.WriteBeginObject();
                foreach ((string storageIndex, string storageValue) in entry.SortedStorage)
                {
                    writer.WriteProperty(storageIndex, storageValue, formatterResolver);
                }

                writer.WriteEndObject();

                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        public GethLikeTxTrace Deserialize(ref Utf8Json.JsonReader reader, IJsonFormatterResolver formatterResolver)
        {
            throw new NotImplementedException();
        }
    }
}