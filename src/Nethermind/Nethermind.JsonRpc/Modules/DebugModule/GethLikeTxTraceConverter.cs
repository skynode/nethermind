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
using System.Text.Json;
using System.Text.Json.Serialization;
using Nethermind.Core.Json;
using Nethermind.Evm.Tracing;

namespace Nethermind.JsonRpc.Modules.DebugModule
{
    public class GethLikeTxTraceConverter : JsonConverter<GethLikeTxTrace>
    {
        private readonly BigIntegerConverter _bigIntegerConverter = new BigIntegerConverter();
        private readonly ByteArrayConverter _byteArrayConverter = new ByteArrayConverter();
        
        public override GethLikeTxTrace Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, GethLikeTxTrace value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartObject();

            writer.WritePropertyName("gas");
            _bigIntegerConverter.Write(writer, value.Gas, options);
            
            writer.WriteBoolean("failed", value.Failed);
            
            writer.WritePropertyName("returnValue");
            _byteArrayConverter.Write(writer, value.ReturnValue, options);


            writer.WritePropertyName("structLogs");
            WriteEntries(writer, value.Entries, options);

            writer.WriteEndObject();
        }
        
        private static void WriteEntries(Utf8JsonWriter writer, List<GethTxTraceEntry> entries, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (GethTxTraceEntry entry in entries)
            {
                writer.WriteStartObject();
                writer.WriteNumber("pc", entry.Pc);
                writer.WriteString("op", entry.Operation);
                writer.WriteNumber("gas", entry.Gas);
                writer.WriteNumber("gasCost", entry.GasCost);
                writer.WriteNumber("depth", entry.Depth);
                writer.WriteString("error", entry.Error);
                writer.WritePropertyName("stack");
                writer.WriteStartArray();
                foreach (string stackItem in entry.Stack)
                {
                    writer.WriteStringValue(stackItem);    
                }
                
                writer.WriteEndArray();
                
                writer.WritePropertyName("memory");
                writer.WriteStartArray();
                foreach (string memory in entry.Memory)
                {
                    writer.WriteStringValue(memory);    
                }
                writer.WriteEndArray();
                
                writer.WritePropertyName("storage");
                writer.WriteStartObject();
                foreach ((string storageIndex, string storageValue) in entry.SortedStorage)
                {
                    writer.WriteString(storageIndex, storageValue);
                }
                
                writer.WriteEndObject();
                
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

    }
}